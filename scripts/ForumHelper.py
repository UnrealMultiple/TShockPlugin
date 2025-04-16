#!/usr/bin/env python3
import json
import re
import sys
import requests as rq
import datetime
from bs4 import BeautifulSoup

name = sys.argv[1] 
password = sys.argv[2]
print(f"论坛自动更新脚本 (by Cai😘)")
print(f"登录名: {name}")

session = rq.Session()

try:
    recent_pr = rq.get("https://api.github.com/repos/UnrealMultiple/TShockPlugin/pulls?state=closed&per_page=1&page=1").json()
    pr_msg = f'<font size="6">✅ <a href="{recent_pr[0]["html_url"]}">{recent_pr[0]["title"]}</a> ({datetime.datetime.strptime(recent_pr[0]["closed_at"], "%Y-%m-%dT%H:%M:%SZ").date()})</font>'
except:
    print(f"获取最新PR失败!")
    exit()

with open('README.md', 'r', encoding='utf-8') as file:
    md = file.read()
md = re.sub(r'\((\./src/([^/]+)/README.md)\)', r'(http://docs.terraria.ink/zh/guide/\2.html)', md)
rendered = rq.post("https://api.github.com/markdown", headers = {
    "Accept": "application/vnd.github+json",
    "X-GitHub-Api-Version": "2022-11-28"
}, data = json.dumps({
    "text": md
})).text

def login(session: rq.Session, url: str):
    resp = session.get(url) 
    soup = BeautifulSoup(resp.text, 'html.parser')
    data_csrf = soup.html['data-csrf']
    session.post(url + "login/login", data = {
        "login": name,
        "password": password,
        "remember": 0,
        "_xfToken": data_csrf,
        "_xfRedirect": url,
    }, allow_redirects = True)
    return data_csrf

try:
    data_csrf = login(session, "https://tr.monika.love/")

    try:
        resp = session.post("https://tr.monika.love/resources/104/edit", data = {
            "prefix_id[]": 7,
            "title": "TShock插件收集仓库(自动更新版)",
            "tag_line": "此帖会自动更新插件列表",
            "version_string": "总是最新",
            "external_download_url": "https://github.moeyy.xyz/https://github.com/UnrealMultiple/TShockPlugin/releases/download/V1.0.0.0/Plugins.zip",
            "description_html": f"{rendered}",
            "external_url": "",
            "icon_action": "custom",
            "_xfRequestUri": "/resources/104/edit",
            "_xfWithData": 1,
            "_xfToken": data_csrf,
            "_xfResponseType": "json"
        })
        res = resp.json()
        if res['status'] == 'ok':
            print(f"[MONIKA] 更新资源成功: {res}")
        else:
            raise Exception(f"{res}")
    except Exception as e:
        print(f"[MONIKA] 更新资源失败! {resp.text} {e}")

    try:
        resp = session.post("https://tr.monika.love/resources/104/post-update", data = {
            "new_update": "1",
            "update_title": "同步仓库更新",
            "update_message_html": pr_msg,
            "_xfRequestUri": "/resources/104/post-update",
            "_xfWithData": "1",
            "_xfToken": data_csrf,
            "_xfResponseType": "json"
        })
        res = resp.json()
        if res['status'] == 'ok':
            print(f"[MONIKA] 添加回复成功: {res}")
        else:
            raise Exception(f"{res}")
    except Exception as e:
        print(f"[MONIKA] 添加回复失败! {resp.text} {e}")
except Exception as e:
    print(f"[MONIKA] 同步仓库更新失败! {e}")

try:
    data_csrf = login(session, "https://trhub.cn/")

    try:
        resp = session.post("https://trhub.cn/posts/107/edit", data = {
            "prefix_id": 0,
            "title": "TShock插件收集仓库(自动更新版)",
            "message_html": f"{rendered}",
            "_xfRequestUri": "/threads/github-action-test.43/",
            "_xfWithData": 1,
            "_xfToken": data_csrf,
            "_xfResponseType": "json"
        })
        res = resp.json()
        if res['status'] == 'ok':
            print(f"[TRHUB] 更新帖子成功: {res}")
        else:
            raise Exception(f"{res}")
    except Exception as e:
        print(f"[TRHUB] 更新帖子失败! {resp.text} {e}")

    try:
        resp = session.post("https://trhub.cn/threads/tshock.43/add-reply", data = {
            "message_html": pr_msg,
            "load_extra": 1,
            "_xfRequestUri": "/threads/tshock.43/",
            "_xfWithData": 1,
            "_xfToken": data_csrf,
            "_xfResponseType": "json"
        })
        res = resp.json()
        if res['status'] == 'ok':
            print(f"[TRHUB] 添加回复成功: {res}")
        else:
            raise Exception(f"{res}")
    except Exception as e:
        print(f"[TRHUB] 添加回复失败! {resp.text} {e}")
except Exception as e:
    print(f"[TRHUB] 同步仓库更新失败! {e}")
