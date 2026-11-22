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

LANG_MAP = {
    'py': 'python', 'js': 'javascript', 'ts': 'typescript',
    'jsx': 'jsx', 'tsx': 'tsx', 'java': 'java', 'cs': 'csharp',
    'cpp': 'cpp', 'c': 'c', 'go': 'go', 'rs': 'rust', 'rb': 'ruby',
    'php': 'php', 'kt': 'kotlin', 'swift': 'swift', 'sh': 'bash',
    'yml': 'yaml', 'yaml': 'yaml', 'json': 'json', 'md': 'markdown',
    'html': 'html', 'css': 'css', 'sql': 'sql', 'xml': 'xml'
}

# Nhóm các ngôn ngữ dùng // và /* */
C_STYLE_EXTS = {'.js', '.ts', '.jsx', '.tsx', '.java', '.cs', '.cpp', '.c', '.go', '.rs', '.kt', '.swift', '.php', '.css'}
# Nhóm các ngôn ngữ dùng #
HASH_STYLE_EXTS = {'.py', '.sh', '.yml', '.yaml', '.rb', '.toml'}

def remove_comments(content: str, ext: str) -> str:
    """Xóa comment an toàn (giữ nguyên chuỗi text chứa // hoặc #)."""
    if ext in C_STYLE_EXTS:
        # Khớp chuỗi ("...", '...', `...`) HOẶC comment (//..., /*...*/)
        pattern = r'("(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|`(?:\\.|[^`\\])*`)|(/\*[\s\(\S\)]*?\*/|//[\(^\r\\)n]*)'
        return re.sub(pattern, lambda m: m.group(1) if m.group(1) else '', content)
    
    elif ext in HASH_STYLE_EXTS:
        # Giữ lại dòng shebang (#!/...) ở đầu file nếu có
        lines = content.splitlines()
        result = []
        pattern = r'("(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\')|(#[\(^\r\\)n]*)'
        for i, line in enumerate(lines):
            if i == 0 and line.startswith('#!'):
                result.append(line)
                continue
            cleaned_line = re.sub(pattern, lambda m: m.group(1) if m.group(1) else '', line)
            result.append(cleaned_line)
        return '\n'.join(result)
        
    elif ext in {'.html', '.xml'}:
        return re.sub(r'<!--[\s\S]*?-->', '', content)
        
    return content

def clean_whitespace(content: str) -> str:
    """Xóa khoảng trắng thừa cuối dòng và gộp dòng trống liên tiếp."""
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
    strip_comments: bool = True,
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
                content = file_path.read_text(encoding='utf-8')
                
                if strip_comments:
                    content = remove_comments(content, ext_lower)
                if compact_lines:
                    content = clean_whitespace(content)
                
                ext_clean = ext_lower.lstrip('.')
                lang = LANG_MAP.get(ext_clean, ext_clean or 'text')
                
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
    print(f"[OK] Đã đóng gói {file_count} file vào '{output_file}' (Xóa comment: {strip_comments}).")
    print(f"[INFO] Tổng dung lượng: {len(final_text):,} ký tự (~{est_tokens:,} tokens ước tính).")

if __name__ == "__main__":
    pack_project(strip_comments=True)
