"""Module Orders: điều phối checkout. Giao tiếp với Cart/Catalog qua hàm public,
với Payments qua sự kiện (không biết Payments tồn tại)."""
from fastapi import APIRouter, HTTPException

from app import events
from app.db import connect
from app.modules import cart, catalog

router = APIRouter(prefix="/api/orders", tags=["orders"])


def init() -> None:
    with connect() as c:
        c.execute("CREATE TABLE IF NOT EXISTS orders (id INTEGER PRIMARY KEY, total INTEGER, status TEXT)")
        c.execute(
            """CREATE TABLE IF NOT EXISTS order_items (
                 order_id INTEGER, product_id INTEGER, name TEXT, unit_price INTEGER, quantity INTEGER)"""
        )
    events.subscribe("payment.succeeded", lambda e: _set_status(e["order_id"], "PAID"))


def _set_status(order_id: int, status: str) -> None:
    with connect() as c:
        c.execute("UPDATE orders SET status = ? WHERE id = ?", (status, order_id))


def _get(order_id: int) -> dict | None:
    with connect() as c:
        order = c.execute("SELECT * FROM orders WHERE id = ?", (order_id,)).fetchone()
        if not order:
            return None
        items = c.execute("SELECT * FROM order_items WHERE order_id = ?", (order_id,)).fetchall()
    return {**dict(order), "items": [dict(i) for i in items]}


@router.post("/checkout/{cart_id}")
def checkout(cart_id: str):
    items = cart.get_items(cart_id)
    if not items:
        raise HTTPException(400, "Giỏ hàng trống")

    lines = []  # (product_id, name, unit_price, quantity)
    for it in items:
        p = catalog.get_product(it["product_id"])
        lines.append((p["id"], p["name"], p["price"], it["quantity"]))
    reservation = [(pid, qty) for pid, _, _, qty in lines]

    try:
        catalog.reserve_stock(reservation)
    except ValueError as e:
        raise HTTPException(409, str(e))

    total = sum(price * qty for _, _, price, qty in lines)
    try:
        with connect() as c:
            order_id = c.execute(
                "INSERT INTO orders (total, status) VALUES (?, 'PENDING')", (total,)
            ).lastrowid
            c.executemany(
                "INSERT INTO order_items VALUES (?, ?, ?, ?, ?)",
                [(order_id, *line) for line in lines],
            )
    except Exception:
        catalog.release_stock(reservation)  # bù trừ: hoàn kho khi tạo đơn lỗi
        raise

    cart.clear(cart_id)
    events.publish("order.placed", {"order_id": order_id, "amount": total})
    return _get(order_id)


@router.get("/{order_id}")
def get_order(order_id: int):
    order = _get(order_id)
    if not order:
        raise HTTPException(404, "Không tìm thấy đơn hàng")
    return order
