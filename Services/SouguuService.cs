using BATTARI_api.Models;

public static class SouguuService {
  /// 関数はuserid
  private static List<Dictionary<UserModel, Func<int>>> _SouguuEventListner =
      new List<Dictionary<UserModel, Func<int>>>();

  /// <summary>
  /// イベントリスナーを追加する
  /// </summary>
  /// <param name="user">ユーザーモデル</param>
  /// <param name="func">関数</param>
  public static void AddSouguuEventListner(UserModel user, Func<int> func) {
    _SouguuEventListner.Add(
        new Dictionary<UserModel, Func<int>> { { user, func } });
  }

  /// <summary>
  /// イベントリスナーを削除する
  /// </summary>
  /// <param name="user">ユーザーモデル</param>
  /// <param name="func">関数</param>
  public static void RemoveSouguuEventListner(UserModel user) {
    _SouguuEventListner.RemoveAll(x => x.ContainsKey(user));
  }
}
