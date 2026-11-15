"""Event bus đồng bộ trong tiến trình. Sau này có thể thay bằng RabbitMQ/Kafka."""
from collections import defaultdict
from typing import Callable

_handlers: dict[str, list[Callable[[dict], None]]] = defaultdict(list)


def subscribe(event: str, handler: Callable[[dict], None]) -> None:
    _handlers[event].append(handler)


def publish(event: str, payload: dict) -> None:
    for handler in _handlers[event]:
        handler(payload)
