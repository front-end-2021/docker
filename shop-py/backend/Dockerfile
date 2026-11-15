# Sử dụng base image Python chính thức
FROM python:3.10-slim

# Đặt thư mục làm việc bên trong container
WORKDIR /code

# Copy file requirements và cài đặt thư viện
COPY requirements.txt .
RUN pip install --no-cache-dir --upgrade -r requirements.txt

# Copy toàn bộ mã nguồn ứng dụng vào container
COPY ./app /code/app

EXPOSE 8000
# Chạy ứng dụng bằng Uvicorn
# Lưu ý: 'main:app' ứng với tên file main.py và biến app trong code của bạn
# Bắt buộc phải là "app.main:app" vì main.py nằm trong thư mục app/
CMD ["uvicorn", "app.main:app", "--host", "0.0.0.0", "--port", "8000"]
