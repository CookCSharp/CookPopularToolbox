# gitlab自动创建MR
### 1. 设置python与sh的环境变量
### 2. 将pre-push文件拷贝到.git/hooks/下
### 3. 将auto_create_mr.py与run.sh拷贝到目标项目文件夹下

## 备注：
1. 每次push代码时会在与auto_create_mr.py同路径下生成user.json文件
2. 每次push代码到远程时就可以自动创建合并请求