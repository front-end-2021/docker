"""Module Catalog: sở hữu bảng catalog_products.
Module khác CHỈ được gọi các hàm trong mục "Public interface"."""
from fastapi import APIRouter

from app.db import connect

router = APIRouter(prefix="/api/products", tags=["catalog"])


def init() -> None:
    with connect() as c:
        c.execute(
            """CREATE TABLE IF NOT EXISTS catalog_products (
                 id INTEGER PRIMARY KEY, name TEXT NOT NULL,
                 price INTEGER NOT NULL, stock INTEGER NOT NULL)"""
        )
        if c.execute("SELECT COUNT(*) FROM catalog_products").fetchone()[0] == 0:
            c.executemany(
                "INSERT INTO catalog_products (name, price, stock) VALUES (?, ?, ?)",
                [
                    ("Bàn phím cơ", 1_290_000, 15),
                    ("Chuột không dây", 450_000, 30),
                    ("Tai nghe chống ồn", 2_490_000, 8),
                    ("Màn hình 24 inch", 3_790_000, 5),
                ],
            )


# ---------- Public interface ----------
def get_product(product_id: int) -> dict | None:
    with connect() as c:
        row = c.execute("SELECT * FROM catalog_products WHERE id = ?", (product_id,)).fetchone()
    return dict(row) if row else None


def reserve_stock(items: list[tuple[int, int]]) -> None:
    """Trừ kho nguyên tử. Ném ValueError nếu thiếu hàng (rollback toàn bộ)."""
    with connect() as c:
        for product_id, qty in items:
            cur = c.execute(
                "UPDATE catalog_products SET stock = stock - ? WHERE id = ? AND stock >= ?",
                (qty, product_id, qty),
            )
            if cur.rowcount == 0:
                raise ValueError(f"Sản phẩm #{product_id} không đủ hàng")


def release_stock(items: list[tuple[int, int]]) -> None:
    with connect() as c:
        c.executemany(
            "UPDATE catalog_products SET stock = stock + ? WHERE id = ?",
            [(qty, product_id) for product_id, qty in items],
        )


# ---------- HTTP ----------
@router.get("")
def list_products():
    with connect() as c:
        return [dict(r) for r in c.execute("SELECT * FROM catalog_products ORDER BY id")]
