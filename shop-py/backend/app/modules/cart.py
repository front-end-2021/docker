"""Module Cart: sở hữu bảng cart_items. Chỉ hỏi Catalog qua hàm public."""
from fastapi import APIRouter, HTTPException
from pydantic import BaseModel, Field

from app.db import connect
from app.modules import catalog

router = APIRouter(prefix="/api/carts", tags=["cart"])


class AddItem(BaseModel):
    product_id: int
    quantity: int = Field(default=1, gt=0)


def init() -> None:
    with connect() as c:
        c.execute(
            """CREATE TABLE IF NOT EXISTS cart_items (
                 cart_id TEXT, product_id INTEGER, quantity INTEGER,
                 PRIMARY KEY (cart_id, product_id))"""
        )


# ---------- Public interface ----------
def get_items(cart_id: str) -> list[dict]:
    with connect() as c:
        rows = c.execute(
            "SELECT product_id, quantity FROM cart_items WHERE cart_id = ?", (cart_id,)
        ).fetchall()
    return [dict(r) for r in rows]


def clear(cart_id: str) -> None:
    with connect() as c:
        c.execute("DELETE FROM cart_items WHERE cart_id = ?", (cart_id,))


# ---------- HTTP ----------
def _view(cart_id: str) -> dict:
    items = []
    for it in get_items(cart_id):
        p = catalog.get_product(it["product_id"])
        items.append({**it, "name": p["name"], "price": p["price"],
                      "line_total": p["price"] * it["quantity"]})
    return {"items": items, "total": sum(i["line_total"] for i in items)}


@router.get("/{cart_id}")
def view_cart(cart_id: str):
    return _view(cart_id)


@router.post("/{cart_id}/items")
def add_item(cart_id: str, body: AddItem):
    if not catalog.get_product(body.product_id):
        raise HTTPException(404, "Không tìm thấy sản phẩm")
    with connect() as c:
        c.execute(
            """INSERT INTO cart_items (cart_id, product_id, quantity) VALUES (?, ?, ?)
               ON CONFLICT (cart_id, product_id)
               DO UPDATE SET quantity = quantity + excluded.quantity""",
            (cart_id, body.product_id, body.quantity),
        )
    return _view(cart_id)


@router.delete("/{cart_id}/items/{product_id}")
def remove_item(cart_id: str, product_id: int):
    with connect() as c:
        c.execute("DELETE FROM cart_items WHERE cart_id = ? AND product_id = ?", (cart_id, product_id))
    return _view(cart_id)
