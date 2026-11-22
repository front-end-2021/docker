import os
import re

# Tên file txt đầu vào của bạn
INPUT_TXT = "taskflow-all-files.txt"
# Thư mục chứa kết quả sau khi tách
OUTPUT_DIR = "Task-flow" 

def split_files():
    if not os.path.exists(INPUT_TXT):
        print(f"Không tìm thấy file: {INPUT_TXT}")
        return

    with open(INPUT_TXT, "r", encoding="utf-8") as f:
        content = f.read()

    # Pattern nhận diện đường dẫn file từ thẻ đánh dấu
    pattern = r"=+\nFILE:\s*(.*?)\n=+\n"
    parts = re.split(pattern, content)

    # parts[0] là phần nội dung trước FILE: đầu tiên (nếu có)
    for i in range(1, len(parts), 2):
        file_path = parts[i].strip()
        file_content = parts[i+1]

        # Đường dẫn đầu ra đầy đủ
        full_out_path = os.path.join(OUTPUT_DIR, file_path)

        # Tự động tạo folder cha nếu chưa tồn tại
        os.makedirs(os.path.dirname(full_out_path), exist_ok=True)

        # Ghi nội dung vào file
        with open(full_out_path, "w", encoding="utf-8") as out_f:
            out_f.write(file_content.strip() + "\n")

        print(f"-> Đã tạo: {file_path}")

    print("\n✅ Tách toàn bộ file thành công!")

if __name__ == "__main__":
    split_files()