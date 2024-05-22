# coding=gbk
# coding:utf-8
import json
import os
import re
import sys

import numpy as np
import requests

# fork项目的详细信息
forks_info = []
project_info = {}
users_info = []

gitlabAccessToken = '**********'  # 访问令牌
header = {'PRIVATE_TOKEN': gitlabAccessToken}

base_api = 'http://gitlab.com/api/v4'
# 查询指定项目
target_project_info_api = f'{base_api}projects'
# 列出所有fork的信息
list_forks_api = f'{base_api}projects/id/forks'
# 创建新分支
create_branch_api = f'{base_api}projects/id/repository/branches'
# 创建合并请求
merge_request_api = f'{base_api}projects/id/merge_requests'
# 获取所有用户
list_all_users_api = f'{base_api}users'


def get(api: str, params: object = None):
    # print(f'api:{api}')
    reply = requests.get(api, headers=header, params=params)
    json_str = reply.text.replace("'", '"')
    json_dict = json.loads(json_str)

    # print(json_str)
    return json_dict


def post(api: str, data: object):
    # print(f'api:{api}')
    reply = requests.get(api, headers=header, data=data)
    json_dict = json.loads(reply.text)


def get_console():
    if len(sys.argv) < 2:
        print('please input mr settings ...')
        exit(0)
    else:
        print(f'length:{len(sys.argv)}')

    mr_str = sys.argv[1].replace("'", '"')
    print(mr_str)
    mr_dict = json.loads(mr_str)

    return mr_dict


def get_project_name_with_path():
    res = os.popen('git remote -v')
    ssh_url = res.read().strip('\n').split('\n')[0].split('\t')[1].replace('(fetch)', '').strip().replace('ssh://', '').replace(':', '/')
    # ssh_url = 'ssh://git@gitee.com:cook-csharp/CookPopularToolbox.git'.replace('ssh://', '').replace(':', '/')

    # 例如：
    # git@gitee.com:cook-csharp/CookPopularToolbox.git
    # https://gitee.com/cook-csharp/CookPopularToolbox.git
    web_url = re.sub(pattern=r'[A-Za-z]+@', repl='https://', string=ssh_url)
    project_name = ssh_url.split('/')[-1].replace('.git', '')
    path_with_namespace = '/'.join(ssh_url.split('/')[-2:]).replace('.git', '')

    print('ssh_url:' + ssh_url)
    print('web_url:' + web_url)
    print('project_name:' + project_name)
    print('path_with_namespace:' + path_with_namespace)

    return {
        'project_name': project_name,
        'path_with_namespace': path_with_namespace
    }


def get_project_info():
    param_pi = {
        'search': project_name,
        'order_by': 'id',
        'sort': 'asc'
    }
    project_dicts = get(target_project_info_api, param_pi)
    for dict_temp in project_dicts:
        if dict_temp['path_with_namespace'] == project_name_with_path['path_with_namespace']:
            global project_info
            project_info = dict_temp
    # get(target_project_info_api+f'?search={project_name}&order_by=id&sort=asc')


def get_all_forks():
    json_dict = get(list_forks_api)
    for dict in json_dict:
        forks_info.append({'id': dict['id'], 'username': dict['owner']['username'], 'web_url': dict['web_url']})

    print(forks_info)


def create_branch():
    param_cb = {
        'id': project_info['id'],
        'brancg': project_info['owner']['username'],
        'ref': 'master'
    }
    branch_api = create_branch_api.replace('id', str(project_info['id']))
    post(branch_api, param_cb)


def create_mr():
    param_mr = {
        'id': project_info['id'],
        'target_project_id': project_info['forked_from_project']['id'],
        'title': mr_dict['title'],
        'description': mr_dict['description'],
        'assignee_id': mr_dict['assignee_id'],
        'reviewer_id': mr_dict['reviewer_id'],
        'source_branch': project_info['owner']['username'],
        'target_branch': 'master',
        'remove_source_branch': True,
        'allow_collaboration': True,
        'squash': True
    }
    merge_api = merge_request_api.replace('id', str(project_info['id']))
    post(merge_api, param_mr)


def get_all_users():
    users = get(list_all_users_api)
    for user in users:
        users_info.append({'id': user['id'], 'username': user['username'], 'email': user['email']})

    print(users_info)
    save_to_file(users_info)


def save_to_file(lst: list):
    keys = [str(x+1) for x in np.arange(len(lst))]
    list_json = dict(zip(keys, lst))
    # json?string
    str_json = json.dumps(list_json, indent=2, ensure_ascii=False)

    f = open('users.json', 'w')
    f.write(str_json)
    f.close()

    return str_json


if __name__ == '__main__':
    # 获取命令行信息
    mr_dict = get_console()

    # 获取项目的web地址
    project_name_with_path = get_project_name_with_path()
    project_name = project_name_with_path['project_name']
    print(project_name_with_path)
    print(project_name)

    # 获取项目信息
    get_project_info()

    # # 列出所有fork的项目信息
    # get_all_forks()

    # 创建新分支
    create_branch()

    # 获取所有用户信息
    get_all_users()

    # 创建合并请求
    create_mr()

# 命令行测试
# python auto_create_mr.py '{'title':'分支合并至master','description':'请求合并代码','assignee_id':3}'
