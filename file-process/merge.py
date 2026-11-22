import os

# Thư mục chứa dự án bạn muốn gộp (ví dụ: 'taskflow_project' hoặc '.')
SOURCE_DIR = "taskflow_project"

# Tên file .txt đầu ra
OUTPUT_TXT = "taskflow-all-files.txt"

# Danh sách các thư mục/file cần BỎ QUA khi gộp (tránh gộp các file rác, bộ nhớ đệm)
EXCLUDE_DIRS = {".git", "node_modules", "dist", "dist-single", ".vscode", "__pycache__"}
EXCLUDE_FILES = {OUTPUT_TXT, "merge.py", "split.py", ".DS_Store", "package-lock.json", "yarn.lock", "pnpm-lock.yaml"}


def merge_files():
    if not os.path.exists(SOURCE_DIR):
        print(f"❌ Không tìm thấy thư mục nguồn: '{SOURCE_DIR}'")
        return

    count = 0
    with open(OUTPUT_TXT, "w", encoding="utf-8") as out_f:
        # Duyệt qua toàn bộ thư mục và thư mục con
        for root, dirs, files in os.walk(SOURCE_DIR):
            # Lọc bỏ các thư mục không muốn duyệt vào
            dirs[:] = [d for d in dirs if d not in EXCLUDE_DIRS]

            for file in sorted(files):
                if file in EXCLUDE_FILES:
                    continue

                full_path = os.path.join(root, file)

                # Lấy đường dẫn tương đối để ghi vào tiêu đề FILE: ...
                rel_path = os.path.relpath(full_path, SOURCE_DIR).replace("\\", "/")

                try:
                    with open(full_path, "r", encoding="utf-8") as in_f:
                        content = in_f.read()

                    # Ghi thẻ phân cách và nội dung vào file txt tổng
                    out_f.write("================================================================================\n")
                    out_f.write(f"FILE: {rel_path}\n")
                    out_f.write("================================================================================\n")
                    out_f.write(content.strip() + "\n\n\n")

                    print(f"-> Đã gộp: {rel_path}")
                    count += 1
                except Exception as e:
                    print(f"⚠️ Bỏ qua file '{rel_path}' (không phải file text hoặc lỗi mã hóa): {e}")

    print(f"\n✅ Thành công! Đã gộp {count} file vào '{OUTPUT_TXT}'.")


if __name__ == "__main__":
    merge_files()