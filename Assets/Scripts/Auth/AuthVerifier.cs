using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AuthVerifier
{
    const string TokenVerifyUrl = "http://localhost:8080/api/auth/verify-access-token";
    const string Authorization = "Authorization";

    public IEnumerator Verify()
    {
        string accessToken = Launcher.AuthToken;
        if (accessToken == null)
        {
            Debug.LogError("AuthVerifier: No Access token found");
            Launcher.ForceQuit();
            yield break;
        }

        UnityWebRequest req = UnityWebRequest.Get(TokenVerifyUrl);
        req.SetRequestHeader(Authorization, $"Bearer {accessToken}");
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success || req.responseCode != 200)
        {
            Debug.LogError("AuthVerifier: Token verifying failed");
            Launcher.ForceQuit();
            yield break;
        }

        Managers.Scene.LoadScene(Define.Scene.Game);
    }
}
