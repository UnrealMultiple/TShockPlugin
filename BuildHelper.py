import os
import glob
import shutil
import sys
import tarfile
from pypandoc import convert_file
import requests
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
    convert_file(input_filename, 'pdf', outputfile=input_filename.replace('.md', '.pdf'))
    

if __name__ == '__main__':
    print(f"😋😋😋打包脚本By Cai...")
    build_type = sys.argv[1]
    print(f"😋开始删除json文件")
    for file in glob.glob(os.path.join(f"out/{build_type}/", "*.json")):
        os.remove(file)
        print(f"删除文件: {file}")
    print("(删除json文件成功~")

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


    # 获取最新的release信息
    response = requests.get('https://api.github.com/repos/jgm/pandoc/releases/latest')
    data = response.json()
    tag_name = data['tag_name']
    tarball_url = data['tarball_url']

    # 下载tar.gz文件
    response = requests.get(tarball_url)
    filename = f'{tag_name}.tar.gz'
    with open(filename, 'wb') as f:
        f.write(response.content)

    # 解压tar.gz文件
    tar = tarfile.open(filename)


    # 创建pandoc文件夹
    if not os.path.exists('pandoc'):
        os.makedirs('pandoc')
    # 解压tar.gz文件到pandoc文件夹
    tar = tarfile.open(filename)
    tar.extractall(path='pandoc')
    tar.close()
    # 删除下载的tar.gz文件
    os.remove(filename)
    # 指定你的目标文件夹
    folder_path = 'pandoc'

    # 获取文件夹中的所有子文件夹
    subfolders = [d for d in os.listdir(folder_path) if os.path.isdir(os.path.join(folder_path, d))]

    # 如果文件夹中只有一个子文件夹
    if len(subfolders) == 1:
        subfolder_path = os.path.join(folder_path, subfolders[0])

        # 遍历子文件夹中的所有文件和子文件夹
        for item in os.listdir(subfolder_path):
            item_path = os.path.join(subfolder_path, item)
            shutil.move(item_path, folder_path)

        # 删除子文件夹
        os.rmdir(subfolder_path)
    os.environ['PATH'] = "pandoc:" + os.environ['PATH']
    for file_name in os.listdir(f"out/{build_type}"):
        if file_name.endswith('.md'):
            md_to_pdf(f"out/{build_type}/"+file_name)
     
    # 调用函数来压缩文件夹中的所有文件
    # 注意：这里需要替换为实际的文件夹路径和zip文件路径
    print("😋准备打包插件")
    zip_files_in_folder("out", "Plugins.zip")
    print("😋😋😋插件打包成功~")






