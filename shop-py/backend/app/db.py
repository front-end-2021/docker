import sqlite3
from contextlib import contextmanager

DB_PATH = "shop.db"


@contextmanager
def connect():
    """Mỗi lần dùng là một transaction: lỗi thì rollback."""
    conn = sqlite3.connect(DB_PATH)
    conn.row_factory = sqlite3.Row
    try:
        yield conn
        conn.commit()
    except Exception:
        conn.rollback()
        raise
    finally:
        conn.close()
