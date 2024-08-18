https://github.com/battari-team/battari

docker
https://hub.docker.com/r/takuto1127/battari-api
# コマンド
## run
`dotnet run {{ポート番号}}`
## データベースマイグレーション
`dotnet ef migrations add {マイグレーションメッセージ}`
then
`dotnet ef database update`
