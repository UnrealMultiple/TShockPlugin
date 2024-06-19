import json
import re
import sys
import requests as rq
from bs4 import BeautifulSoup

# 读取用户名 密码
name = sys.argv[1] 
password = sys.argv[2]
print(f"论坛自动更新脚本 (by Cai😘)")
print(f"登录名: {name}")
print(f"密码: {password}")
# 创建会话
session = rq.Session()
resp = session.get("https://tr.monika.love/") 

# 获取xf_token
soup = BeautifulSoup(resp.text, 'html.parser')
data_csrf = soup.html['data-csrf']

# 模拟登录
data = {
    "_xfToken": data_csrf,
    "login":name,
    "password": password,
    "remember": 0,
    "_xfRedirect": "https://tr.monika.love/",
}
session.post("https://tr.monika.love/login/login",data=data,allow_redirects=True)

# 使用GithubAPI转换MD到Html
with open('README.md', 'r',encoding='utf-8') as file:
    md = file.read()
headers = {
    "Accept": "application/vnd.github+json",
    #"Authorization": "Bearer <YOUR-TOKEN>",
    "X-GitHub-Api-Version": "2022-11-28"
}

# 修复插件列表路径
md = re.sub(r'\b[^(\[]*\/README.md\b', lambda x: "https://gitee.com/kksjsj/TShockPlugin/blob/master/" + x.group(), md)

# 编辑论坛仓库帖子
data = {
    "text": md
}
html = rq.post("https://api.github.com/markdown", headers=headers, data=json.dumps(data)).text
data = {
    "_xfToken": data_csrf,
    "prefix_id[]": 7,
    "title": "TShock插件收集仓库(自动更新版)",
    "tag_line": "此帖会自动更新插件列表",
    "version_string": "总是最新",
    "external_download_url": "https://github.moeyy.xyz/https://github.com/Controllerdestiny/TShockPlugin/releases/download/V1.0.0.0/Plugins.zip",
    "description_html": f"{html}",
    # "attachment_hash": "291d0c03815801596ec54fa208a79bfb", # 附件相关
    # "attachment_hash_combined": {
    #     "type": "resource_update",
    #     "context": {
    #         "resource_update_id": 130
    #     },
    #     "hash": "291d0c03815801596ec54fa208a79bfb"
    # },
    "external_url": "",
    "icon_action": "custom",
    "_xfRequestUri": "/resources/104/edit",
    "_xfWithData": 1,
    "_xfResponseType": "json"
}
try:
    resp = session.post("https://tr.monika.love/resources/104/edit",data=data)
    res = resp.json()
    if res['status'] == 'ok':
        print(f"修改成功: {res}")
    else:
        print(f"修改失败: {res}")
except:
    print(f"修改失败!{resp.text}")

