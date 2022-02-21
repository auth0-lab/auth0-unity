using Auth0;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SignInMain : MonoBehaviour
{
    private void OnEnable()
    {
        this.UpdateLoginStatus();
    }

    public void SignOutBtn()
    {
        AuthManager.Instance.Credentials.ClearCredentials();
        this.UpdateLoginStatus();
    }

    private void UpdateLoginStatus()
    {
        var loggedIn = AuthManager.Instance.Credentials.HasValidCredentials();

        if (loggedIn)
        {
            FindObject(gameObject, "SignInBtn").SetActive(false);
            FindObject(gameObject, "SignOutBtn").SetActive(true);
        }
        else
        {
            FindObject(gameObject, "SignInBtn").SetActive(true);
            FindObject(gameObject, "SignOutBtn").SetActive(false);
        }
    }

    public static GameObject FindObject(GameObject parent, string name)
    {
        Transform[] trs= parent.GetComponentsInChildren<Transform>(true);

        foreach (Transform t in trs)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }
        }

        return null;
    }
}
