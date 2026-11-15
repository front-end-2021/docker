"""Module Payments: lắng nghe order.placed, phát payment.succeeded."""
from app import events
from app.db import connect


def init() -> None:
    with connect() as c:
        c.execute(
            "CREATE TABLE IF NOT EXISTS payments (id INTEGER PRIMARY KEY, order_id INTEGER, amount INTEGER, status TEXT)"
        )
    events.subscribe("order.placed", _on_order_placed)


def _on_order_placed(event: dict) -> None:
    # Giả lập cổng thanh toán (luôn thành công). Thực tế: gọi VNPay/MoMo/Stripe tại đây.
    with connect() as c:
        c.execute(
            "INSERT INTO payments (order_id, amount, status) VALUES (?, ?, 'SUCCEEDED')",
            (event["order_id"], event["amount"]),
        )
    events.publish("payment.succeeded", {"order_id": event["order_id"]})
