import sys
import requests

gitee_token = sys.argv[1]
owner = 'kksjsj'
repo = 'TShockPlugin'
release_id = 431070
headers = {
    'Authorization': f'token {gitee_token}'
}
files = {
    'file': open('out/Plugins.zip', 'rb')
}
attachments = requests.get(f"https://gitee.com/api/v5/repos/{owner}/{repo}/releases/{release_id}/attach_files?&page=1&per_page=100&direction=asc",headers=headers).json()
for attachment in attachments:
    response = requests.delete(f"https://gitee.com/api/v5/repos/{owner}/{repo}/releases/{release_id}/attach_files/{attachment['id']}",headers=headers)
    print(f'🗑️ 附件 {attachment["name"]} 删除{"成功" if response.status_code == 204 else "失败"}：{response.status_code}')

response = requests.post(f"https://gitee.com/api/v5/repos/{owner}/{repo}/releases/{release_id}/attach_files", headers=headers, files=files)
if response.status_code == 201:
    print('✨ Gitee插件包上传成功！')
    print('下载链接:', response.json()['browser_download_url'])
else:
    print('❓Gitee插件包上传失败:', response.status_code)
    print(response.json())