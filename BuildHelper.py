import os
import glob
import shutil
import sys
from pypandoc import convert_file
import pypandoc
import zipfile

def zip_files_in_folder(folder_path, zip_file_path):
    # Create a ZipFile object in write mode
    with zipfile.ZipFile(zip_file_path, 'w') as zipf:
        # Iterate over each file in the folder
        for foldername, subfolders, filenames in os.walk(folder_path):
            for filename in filenames:
                # Create the complete filepath by concatenating the folder and filename
                file_path = os.path.join(foldername, filename)
                # Add file to the zip file
                # The arcname parameter is used to store the file without any folder structure
                zipf.write(file_path, arcname=os.path.basename(file_path))
    print(f"生成压缩包: {zip_file_path}")

def md_to_pdf(input_filename):
    print(input_filename)
    convert_file(input_filename, 'pdf', outputfile=input_filename.replace('.md', '.pdf'),extra_args=[
        '--pdf-engine=xelatex',  # 设置 LaTeX 引擎为 xelatex
        '-V', 'mainfont:Noto Serif SC',  # 设置主字体为 Noto Serif CJK SC
        '-V', 'sansfont:Noto Sans SC', 
        '-V', 'monofont:Noto Sans Mono'

    ],)
    

if __name__ == '__main__':
    print(f"😋😋😋打包脚本By Cai...")
    build_type = sys.argv[1]
    print(f"😋开始删除json文件")
    for file in glob.glob(os.path.join(f"out/{build_type}/", "*.json")):
        os.remove(file)
        print(f"删除文件: {file}")
    print("删除json文件成功~")

    # Get the current working directory
    print("😋开始移动README.md")
    cwd = os.getcwd()
    #shutil.copyfile("README.md",f"out/{build_type}/README.md")
    # Iterate over all directories in the current working directory
    for dir_name in os.listdir(cwd):
        dir_path = os.path.join(cwd, dir_name)
        # Check if it is a directory
        if os.path.isdir(dir_path):
            # Iterate over all files in the directory
            for file_name in os.listdir(dir_path):
                # Check if the file is a .csproj file
                try:
                    if file_name.endswith('.csproj'):
                        # Construct the source path of the README.md file
                        source_path = os.path.join(dir_path, 'README.md')
                        # Construct the destination path in the out/{build_type} directory with the same name as the .csproj file
                        destination_path = os.path.join(cwd, 'out', f'{build_type}', file_name.replace('.csproj', '.md'))
                        # Copy the README.md file to the destination path
                        shutil.copyfile(source_path, destination_path)
                        print(f"找到README.md({destination_path})")
                except:
                    print(f"README移动失败({file_name})")
    print("移动README.md成功~")

    pypandoc.download_pandoc()

    for file_name in os.listdir(f"out/{build_type}"):
        if file_name.endswith('.md'):
            md_to_pdf(os.path.join(f"out/{build_type}", file_name))
    
     
    # 调用函数来压缩文件夹中的所有文件
    # 注意：这里需要替换为实际的文件夹路径和zip文件路径
    print("😋准备打包插件")
    zip_files_in_folder("out", "Plugins.zip")
    print("😋😋😋插件打包成功~")






