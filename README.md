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

## データベースマイグレーション
`dotnet ef migrations add {マイグレーションメッセージ}`

then（実際にデータベースに反映する）

`dotnet ef database update`
