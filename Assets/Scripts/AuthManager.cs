using System;
using Apple.GameKit;
using Plugins.Social.Extension;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.UI;

namespace Auth
{
    public class AuthManager : MonoBehaviour
    {
        public Text NicknameText;
        public Text IdentityText;
        
        public Button LoginPluginButton;
        public Button LoginSocialButton;
        public Button GetIdentityPluginButton;
        public Button GetIdentitySocialButton;

        private void Start()
        {
            Social.Active = new GameCenterPlatform();

            LoginPluginButton.onClick.AddListener(LoginFromPlugin);
            LoginSocialButton.onClick.AddListener(LoginFromSocial);
            GetIdentityPluginButton.onClick.AddListener(GetIdentityPlugin);
            GetIdentitySocialButton.onClick.AddListener(GetIdentitySocial);
        }

        private async void LoginFromPlugin()
        {
            try
            {
                Debug.Log("Start Authentication AppleGameKit");
                LoginPluginButton.interactable = false;

                var player = await GKLocalPlayer.Authenticate();
                Debug.Log($"GameKit Authentication: isAuthenticated => {player.IsAuthenticated}");

                if (player.IsAuthenticated)
                {
                    NicknameText.text = player.DisplayName;

                    Debug.Log($"Local Player: {player.DisplayName}");
                    Debug.Log("Complete Authentication AppleGameKit");
                }
                else
                {
                    Debug.LogError("Failed Authentication AppleGameKit");
                }
            }
            catch (GameKitException ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                LoginPluginButton.interactable = true;
            }
        }

        private async void GetIdentityPlugin()
        {
            try
            {
                Debug.LogWarning("Identity GameKit");
                GetIdentityPluginButton.interactable = false;

                var fetchItemsResponse = await GKLocalPlayer.Local.FetchItems();
                var signature = Convert.ToBase64String(fetchItemsResponse.GetSignature());
                var salt = Convert.ToBase64String(fetchItemsResponse.GetSalt());
                var publicKeyUrl = fetchItemsResponse.PublicKeyUrl;
                var timestamp = fetchItemsResponse.Timestamp.ToString();
                
                Debug.Log($"GameKit Authentication: signature => {signature}");
                Debug.Log($"GameKit Authentication: publickeyurl => {publicKeyUrl}");
                Debug.Log($"GameKit Authentication: salt => {salt}");
                Debug.Log($"GameKit Authentication: Timestamp => {timestamp}");
                
                IdentityText.text = $"signature => {signature}\n" +
                                    $"publickeyurl => {publicKeyUrl}\n" +
                                    $"salt => {salt}\n" +
                                    $"Timestamp => {timestamp}";

                Debug.Log("Complete Identity GameKit");
            }
            catch (GameKitException ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                GetIdentityPluginButton.interactable = true;
            }
        }

        private void LoginFromSocial()
        {
            try
            {
                Debug.Log("Start Authentication Social");
                LoginSocialButton.interactable = false;
                
                Social.localUser.Authenticate(success =>
                {
                    var player = Social.localUser;
                    Debug.Log($"Social Authentication: isAuthenticated => {player.authenticated}");
                    
                    if (success)
                    {
                        NicknameText.text = player.userName;
                        
                        Debug.Log("Complete Authentication Social");
                        Debug.Log($"Local Player: {player.userName}");
                    }
                    else
                    {
                        Debug.LogError("Failed Authentication Social");
                    }
                    
                    LoginSocialButton.interactable = true;
                });
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
        private void GetIdentitySocial()
        {
            try
            {
                Debug.Log("Start Identity Social");
                GetIdentitySocialButton.interactable = false;
                
                IdentityVerificationSignature.UpdateIdentity += UpdateIdentity;
                
                IdentityVerificationSignature.generateIdentityVerificationSignature(IdentityVerificationSignature
                    .OnIdentityVerificationSignatureGenerated);

                void UpdateIdentity()
                {
                    var signature = Convert.ToBase64String(IdentityVerificationSignature.Signature);
                    var salt = Convert.ToBase64String(IdentityVerificationSignature.Signature);
                    var publicKeyUrl = IdentityVerificationSignature.PublicKeyUrl;
                    var timestamp = IdentityVerificationSignature.Timestamp;
                    var error = IdentityVerificationSignature.Error;
                    
                    Debug.Log($"Social Authentication: signature => {signature}");
                    Debug.Log($"Social Authentication: publickeyurl => {publicKeyUrl}");
                    Debug.Log($"Social Authentication: salt => {salt}");
                    Debug.Log($"Social Authentication: Timestamp => {timestamp}");
                    Debug.Log($"Social Authentication: Error => {error}");
                    
                    // IdentityText.text = $"signature => {signature}\n" +
                    //                     $"publickeyurl => {publicKeyUrl}\n" +
                    //                     $"salt => {salt}\n" +
                    //                     $"Timestamp => {timestamp}\n" +
                    //                     $"Error => {error}";

                    Debug.Log("Complete Identity Social");
                    
                    GetIdentitySocialButton.interactable = true;

                    IdentityVerificationSignature.UpdateIdentity -= UpdateIdentity;
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}
