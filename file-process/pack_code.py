import os
import re
from pathlib import Path

# 1. CẤU HÌNH BỎ QUA
IGNORE_DIRS = {
    '.git', '.svn', '.hg', '__pycache__', 'node_modules', 'venv', '.venv', 
    'env', 'dist', 'build', '.next', '.nuxt', '.idea', '.vscode', 'target', 
    'bin', 'obj', 'vendor', 'coverage', '.pytest_cache', '.mypy_cache'
}

IGNORE_EXTENSIONS = {
    '.png', '.jpg', '.jpeg', '.gif', '.ico', '.svg', '.webp', '.pdf',
    '.zip', '.tar', '.gz', '.rar', '.7z', '.exe', '.dll', '.so', '.dylib',
    '.pyc', '.pyo', '.class', '.jar', '.war', '.mp3', '.mp4', '.wav',
    '.woff', '.woff2', '.ttf', '.eot', '.db', '.sqlite', '.sqlite3', '.lock',
    '.map', '.min.js', '.min.css'
}

IGNORE_FILES = {
    'package-lock.json', 'yarn.lock', 'pnpm-lock.yaml', 'Cargo.lock', 
    'poetry.lock', 'Gemfile.lock', 'composer.lock', '.DS_Store', 'Thumbs.db',
    'pack_code.py', 'project_context.md', 'project_context.txt'
}

# Ánh xạ đuôi file sang tên ngôn ngữ Markdown để Claude nhận diện chuẩn xác
LANG_MAP = {
    'py': 'python', 'js': 'javascript', 'ts': 'typescript',
    'jsx': 'jsx', 'tsx': 'tsx', 'java': 'java', 'cs': 'csharp',
    'cpp': 'cpp', 'c': 'c', 'go': 'go', 'rs': 'rust', 'rb': 'ruby',
    'php': 'php', 'kt': 'kotlin', 'swift': 'swift', 'sh': 'bash',
    'yml': 'yaml', 'yaml': 'yaml', 'json': 'json', 'md': 'markdown',
    'html': 'html', 'css': 'css', 'sql': 'sql', 'xml': 'xml'
}

def clean_whitespace(content: str) -> str:
    """Rút gọn các dòng trống liên tiếp (từ 3 dòng trở lên thành 2) để tiết kiệm token."""
    lines = [line.rstrip() for line in content.splitlines()]
    cleaned = "\n".join(lines)
    return re.sub(r'\n{3,}', '\n\n', cleaned).strip()

def generate_tree(root_dir: Path) -> str:
    tree_lines = [f"{root_dir.name}/"]
    for root, dirs, files in os.walk(root_dir):
        dirs[:] = sorted([d for d in dirs if d not in IGNORE_DIRS and not d.startswith('.')])
        files = sorted([
            f for f in files 
            if f not in IGNORE_FILES and not f.startswith('.') 
            and Path(f).suffix.lower() not in IGNORE_EXTENSIONS
        ])
        
        rel_path = Path(root).relative_to(root_dir)
        level = 0 if rel_path == Path('.') else len(rel_path.parts)
        indent = "    " * level
        
        if rel_path != Path('.'):
            tree_lines.append(f"{indent}├── {rel_path.name}/")
            sub_indent = "    " * (level + 1)
        else:
            sub_indent = "    "
            
        for f in files:
            tree_lines.append(f"{sub_indent}├── {f}")
    return "\n".join(tree_lines)

def pack_project(
    root_path: str = ".", 
    output_file: str = "project_context.md",
    compact_lines: bool = True
):
    root_dir = Path(root_path).resolve()
    tree_str = generate_tree(root_dir)
    
    packed_content = [
        f"# Project Context: {root_dir.name}\n",
        "## 1. Cấu trúc thư mục",
        "```text",
        tree_str,
        "```\n",
        "## 2. Chi tiết mã nguồn\n"
    ]
    
    file_count = 0
    for root, dirs, files in os.walk(root_dir):
        dirs[:] = sorted([d for d in dirs if d not in IGNORE_DIRS and not d.startswith('.')])
        files = sorted([f for f in files if f not in IGNORE_FILES and not f.startswith('.')])
        
        for file in files:
            file_path = Path(root) / file
            ext_lower = file_path.suffix.lower()
            if ext_lower in IGNORE_EXTENSIONS or file.endswith(('.min.js', '.min.css')):
                continue
                
            rel_file = file_path.relative_to(root_dir)
            try:
                raw_content = file_path.read_text(encoding='utf-8')
                content = clean_whitespace(raw_content) if compact_lines else raw_content
                
                ext_clean = ext_lower.lstrip('.')
                lang = LANG_MAP.get(ext_clean, ext_clean or 'text')
                
                # Dùng 4 dấu nháy ngược (````) để tránh xung đột nếu file chứa sẵn block code ```
                packed_content.append(f"### `{rel_file.as_posix()}`")
                packed_content.append(f"````{lang}")
                packed_content.append(content)
                packed_content.append("````\n")
                file_count += 1
            except UnicodeDecodeError:
                continue

    final_text = "\n".join(packed_content)
    Path(output_file).write_text(final_text, encoding='utf-8')
    
    est_tokens = len(final_text) // 4
    print(f"[OK] Đã đóng gói {file_count} file vào '{output_file}'.")
    print(f"[INFO] Tổng dung lượng: {len(final_text):,} ký tự (~{est_tokens:,} tokens ước tính).")
    if est_tokens > 50000:
        print("[CẢNH BÁO] Lượng token khá lớn (>50k), bạn nên cân nhắc lọc bớt thư mục không cần thiết.")

if __name__ == "__main__":
    pack_project()
