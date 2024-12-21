https://github.com/battari-team/battari

docker
https://hub.docker.com/r/takuto1127/battari-api
# コマンド
## run
`dotnet run {{ポート番号}}`
or
`docker run -p {{ポート番号}}:{{ポート番号}} takuto1127/battari-api` {{ポート番号}}

## ビルド
`docker build -t takuto1127/battari-api:{{バージョン}} .`

## 開発におけるデータベースの立ち上げ
`docker run --name mariadb-container -d \
     -e MYSQL_ROOT_PASSWORD=battarikun1127 \
     -e MYSQL_DATABASE=battari \
     -p 3306:3306 \
     mariadb:latest`

## データベースマイグレーション
`dotnet ef migrations add {マイグレーションメッセージ}`

then（実際にデータベースに反映する）

`dotnet ef database update`
