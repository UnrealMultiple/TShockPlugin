import datetime
import json
import sys
from bs4 import BeautifulSoup
import requests as rq

result = rq.get("https://api.github.com/repos/Controllerdestiny/TShockPlugin/pulls?state=closed&per_page=1&page=1").json()


if result[0]['merged_at'] is None:
    print("未合并,跳过发送")
    exit(0)

        

html =f'<font size="6">✅ [{result[0]["title"]}]({result[0]["html_url"]}) ({datetime.datetime.strptime(result[0]["closed_at"], "%Y-%m-%dT%H:%M:%SZ").date()})</font>'
# 读取用户名 密码
name = sys.argv[1] 
password = sys.argv[2]
print(f"论坛自动更新脚本 (by Cai😘)")
print(f"登录名: {name}")
print(f"密码: {password}")


# tr.monika.love
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


data = {
    "new_update": "1",
	"update_title": "同步仓库更新",
	"update_message_html": f"{html}",
	#"attachment_hash": "ed8d3a4157b31fcf4911bfaf14fb7300",
	#"attachment_hash_combined": "{\"type\":\"resource_update\",\"context\":{\"resource_id\":115},\"hash\":\"ed8d3a4157b31fcf4911bfaf14fb7300\"}",
	"_xfRequestUri": "/resources/115/post-update",
	"_xfWithData": "1",
	"_xfToken": data_csrf,
	"_xfResponseType": "json"
}
try:
    resp = session.post("https://tr.monika.love/resources/115/post-update",data=data)
    res = resp.json()
    if res['status'] == 'ok':
        print(f"[MONIKA]修改成功: {res}")
    else:
        print(f"[MONIKA]修改失败: {res}")
except:
    print(f"[MONIKA]修改失败!{resp.text}")


# trhub.cn
# 创建会话
session = rq.Session()
resp = session.get("https://trhub.cn/") 

# 获取xf_token
soup = BeautifulSoup(resp.text, 'html.parser')
data_csrf = soup.html['data-csrf']

# 模拟登录
data = {
    "_xfToken": data_csrf,
    "login":name,
    "password": password,
    "remember": 0,
    "_xfRedirect": "https://trhub.cn/",
}
session.post("https://trhub.cn/login/login",data=data,allow_redirects=True)


data = {
    "_xfToken": data_csrf,
    "message_html": f"{html}",
    # "attachment_hash": "291d0c03815801596ec54fa208a79bfb", # 附件相关
    # "attachment_hash_combined": {
    #     "type": "resource_update",
    #     "context": {
    #         "resource_update_id": 130
    #     },
    #     "hash": "291d0c03815801596ec54fa208a79bfb"
    # },
    "load_extra": 1,
    "_xfRequestUri": "/threads/tshock.43/",
    "_xfWithData": 1,
    "_xfResponseType": "json"
}
try:
    resp = session.post("https://trhub.cn/threads/tshock.43/add-reply",data=data)
    res = resp.json()
    if res['status'] == 'ok':
        print(f"[TRHUB]修改成功: {res}")
    else:
        print(f"[TRHUB]修改失败: {res}")
except:
    print(f"[TRHUB]修改失败!{resp.text}")

# BBSTR
name = "Cai233"
# 创建会话
session = rq.Session()
resp = session.get("https://tr.lizigo.cn/") 

# 获取xf_token
soup = BeautifulSoup(resp.text, 'html.parser')
data_csrf = soup.html['data-csrf']
 

# 模拟登录
data = {
    "_xfToken": data_csrf,
    "login":name,
    "password": password,
    "remember": 0,
    "_xfRedirect": "https://tr.lizigo.cn/",
}
session.post("https://tr.lizigo.cn/login/login",data=data,allow_redirects=True)

# 模拟登录

data = {
    "_xfToken": data_csrf,
    "message_html": f"{html}",
    # "attachment_hash": "291d0c03815801596ec54fa208a79bfb", # 附件相关
    # "attachment_hash_combined": {
    #     "type": "resource_update",
    #     "context": {
    #         "resource_update_id": 130
    #     },
    #     "hash": "291d0c03815801596ec54fa208a79bfb"
    # },
    "load_extra": 1,
    "_xfRequestUri": "/threads/2427/",
    "_xfWithData": 1,
    "_xfResponseType": "json"
}
try:
    resp = session.post("https://tr.lizigo.cn/threads/2427/add-reply",data=data)
    res = resp.json()
    if res['status'] == 'ok':
        print(f"[BBSTR]修改成功: {res}")
    else:
        print(f"[BBSTR]修改失败: {res}")
except:
    print(f"[BBSTR]修改失败!{resp.text}")
