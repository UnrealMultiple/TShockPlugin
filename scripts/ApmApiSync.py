#!/usr/bin/env python3
import sys
import requests

token = sys.argv[1]
payload = {
    'token': f'{token}'
}
files = {
    'file': open('out/Plugins.zip', 'rb')
}

response = requests.post(f"http://api.terraria.ink:11434/plugin/upload", data=payload, files=files)
if response.status_code == 200:
    print('✨ ApmApi插件包上传成功！')
else:
    print('❓ApmApi插件包上传失败:', response.status_code)
    print(response.text)