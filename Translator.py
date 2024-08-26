import os
import subprocess

cmd = ["sudo", "MarkdownTranslator/MarkdownTranslator.exe", "-f"]
for file in os.listdir("./"):
    path = f"{file}/README.md"
    if os.path.exists(path):
      cmd.append(file)
subprocess.run(cmd)
