import os
import glob
import shutil
import sys
import zipfile

def zip_files_in_folder(folder_path, zip_file_path):
    with zipfile.ZipFile(zip_file_path, 'w', compression=zipfile.ZIP_DEFLATED, compresslevel=9) as zipf:
        for foldername, subfolders, filenames in os.walk(folder_path):
            for filename in filenames:
                file_path = os.path.join(foldername, filename)
                arcname = os.path.relpath(file_path, folder_path)
                zipf.write(file_path, arcname=arcname)
    print(f"📦 压缩包已生成: {zip_file_path}")
    

if __name__ == '__main__':
    print(f"🚀 开始执行打包脚本...(By Cai 😋)")
    build_type = sys.argv[1]

    print(f"🗑️ 开始删除json文件...")
    for file in glob.glob(os.path.join(f"out/{build_type}/", "*.json")):
        os.remove(file)
        print(f"✅ 已删除文件: {file}")
    print("✅ json文件删除成功！")
    
    print("📝 开始移动README.md...")
    cwd = os.getcwd()
    for dir_name in os.listdir(cwd):
        dir_path = os.path.join(cwd, dir_name)
        if os.path.isdir(dir_path):
            for file_name in os.listdir(dir_path):
                try:
                    if file_name.endswith('.csproj'):
                        source_path = os.path.join(dir_path, 'README.md')
                        destination_path = os.path.join(cwd, 'out', f'{build_type}', file_name.replace('.csproj', '.md'))
                        shutil.copyfile(source_path, destination_path)
                        print(f"🔍 找到README.md({destination_path})")
                except:
                    print(f"⚠️ README移动失败({file_name})")


    os.makedirs(f'out/{build_type}/Plugins', exist_ok=True)

    out_dir = f'out/{build_type}'
    files = [f for f in os.listdir(out_dir) if os.path.isfile(os.path.join(out_dir, f))]


    for file in files:
        shutil.move(os.path.join(out_dir, file), os.path.join(f'out/{build_type}/Plugins', file))
                    
    shutil.copyfile('README.md', f"out/{build_type}/TShockPlugin.md")
    print("✅ README.md移动成功！")

    shutil.copyfile('Usage.txt', f"out/{build_type}/使用前须知.txt")
    print("✅ 使用前须知.txt移动成功！")

    shutil.copyfile('Plugins.json', f"out/{build_type}/Plugins.json")
    print("✅ Plugins.json移动成功！")

    shutil.copyfile('LICENSE', f"out/{build_type}/LICENSE")
    print("✅ 开源协议移动成功！")

    print("📦 准备打包插件...")
    zip_files_in_folder("out/{build_type}", "Plugins.zip")
    print("🎉 插件打包成功！")
