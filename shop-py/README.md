# Mini Shop: modular monolith (FastAPI) + Vue 3

## Chạy thử
```bash
# Terminal 1: backend (Python 3.10+)
cd backend
python -m venv .venv && source .venv/bin/activate
pip install -r requirements.txt
uvicorn app.main:app --reload          # http://localhost:8000/docs

# Terminal 2: frontend
cd frontend
npm install && npm run dev             # http://localhost:5173
```

## Kiến trúc
```
Vue 3 ──HTTP──▶ FastAPI (một tiến trình)
                 ├─ catalog   (sản phẩm, tồn kho)
                 ├─ cart      (giỏ hàng)      ──gọi hàm──▶ catalog
                 ├─ orders    (checkout)      ──gọi hàm──▶ cart, catalog
                 └─ payments  (thanh toán giả lập)
orders ──order.placed──▶ payments ──payment.succeeded──▶ orders
```

## Luật của dự án
1. Mỗi module sở hữu bảng của mình (tiền tố `catalog_`, `cart_`...). **Không JOIN, không đọc bảng của module khác.**
2. Module chỉ gọi nhau qua hàm trong mục "Public interface".
3. Việc không cần kết quả ngay (thanh toán) đi qua sự kiện, để module gửi không biết module nhận.

## Thử nghiệm để học
- Đổi `reserve_stock` để thấy điều gì xảy ra khi hai người mua cùng lúc món cuối cùng.
- Làm `payments` ném lỗi: đơn hàng và kho sẽ ở trạng thái nào? Thêm `payment.failed` và hoàn kho.
- Thay `events.py` bằng queue thật (Redis/RabbitMQ) và tách `payments` thành service riêng. Bạn sẽ gặp idempotency, retry và outbox pattern.
- Chưa có xác thực: thêm đăng nhập, gắn giỏ hàng và đơn hàng với user.
