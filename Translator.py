import os
import subprocess
import shutil

os.remove("MarkdownTranslator/config.yaml")
shutil.copy("Config/config.yaml","MarkdownTranslator/config.yaml")
shutil.copy("Config/config.yaml","./config.yaml")
for file in os.listdir("MarkdownTranslator"):
    print(file)
cmd = ["MarkdownTranslator/MarkdownTranslator.exe", "-f"]
for file in os.listdir("./"):
    path = f"{file}/README.md"
    _path = f"{file}/README.en.md"
    if os.path.exists(_path):
        os.remove(_path)
    if os.path.exists(path):
      cmd.append(file)
subprocess.run(cmd)

subprocess.run(["git", "config", "--local", "user.email", "action@github.com"])
subprocess.run(["git", "config", "--local", "user.name", "GitHub Action"])

for file in os.listdir("./"):
   path = f"{file}/README.en.md"
   if(os.path.exists(path)):
      subprocess.run(["git", "add", path])

subprocess.run(["git", "commit", "-m", "自动更新README.md"])
subprocess.run(["git", "pull"])
subprocess.run(["git", "push"])
