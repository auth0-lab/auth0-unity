using Auth0;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainSectionController : MonoBehaviour
{
    private void OnEnable()
    {
        this.UpdateLoginStatus();
    }

    public void SignOutBtn()
    {
        AuthManager.Instance.credentials.ClearCredentials();
        this.UpdateLoginStatus();
    }

    public void ExitBtn()
    {
        Application.Quit();
    }

    private void UpdateLoginStatus()
    {
        var loggedIn = AuthManager.Instance.credentials.HasValidCredentials();

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
