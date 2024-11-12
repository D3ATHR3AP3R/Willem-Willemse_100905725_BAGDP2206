using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationHandler
{
    public static AuthState authState { get; private set; } = AuthState.NotAuthenticated;

    public static async Task<AuthState> DoAuth(int maxTries = 5)
    {
        if (authState == AuthState.Authenticated)
        {
            return (authState);
        }

        if (authState == AuthState.Authenticating)
        {
            Debug.LogWarning("Already Authenticating");
            await Authenticating();
            return (authState);
        }

        await SignInAnonymouslyAsinc(maxTries);

        return (authState);
    }

    private static async Task<AuthState> Authenticating()
    {
        while (authState == AuthState.Authenticating || authState == AuthState.NotAuthenticated)
        {
            await Task.Delay(250);
        }

        return (authState);
    }

    private static async Task SignInAnonymouslyAsinc(int maxTries)
    {
        authState = AuthState.Authenticating;

        int tries = 0;
        while (authState == AuthState.Authenticating && tries < maxTries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    authState = AuthState.Authenticated;
                }
            }
            catch (AuthenticationException authException)
            {
                Debug.LogError(authException);
                authState = AuthState.Error;
            }
            catch (RequestFailedException requestException)
            {
                Debug.LogError(requestException);
                authState |= AuthState.Error;
            }

            tries++;

            await Task.Delay(1000);
        }

        if (authState != AuthState.Authenticated)
        {
            Debug.LogWarning($"Player was not signed in successfully after {tries} retries");
            authState = AuthState.TimeOut;
        }
    }
}

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut
}
