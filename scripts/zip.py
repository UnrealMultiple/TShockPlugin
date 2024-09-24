import os
import zipfile
import sys

platform = sys.argv[1]
folder_path = "out/Target"
zip_file_path = f"out/Plugins_{platform}.zip"

with zipfile.ZipFile(zip_file_path, 'w', compression=zipfile.ZIP_DEFLATED, compresslevel=9) as zipf:
    for foldername, subfolders, filenames in os.walk(folder_path):
        for filename in filenames:
            file_path = os.path.join(foldername, filename)
            arcname = os.path.relpath(file_path, folder_path)
            zipf.write(file_path, arcname=arcname)
print(f"📦 压缩包已生成: {zip_file_path}")