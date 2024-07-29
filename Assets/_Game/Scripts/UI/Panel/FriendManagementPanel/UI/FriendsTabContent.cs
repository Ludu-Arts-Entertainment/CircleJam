using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FriendsTabContent : MonoBehaviour
{
    public List<FriendshipStatus> friendshipStatus;
    private readonly List<FriendsTab> _unusedFriendTabs = new();
    private readonly Dictionary<string, FriendsTab> _usingFriendsTab = new ();
    
    private List<FriendInfoModel> _friendModels = new ();
    private async void OnEnable()
    {
        GameInstaller.Instance.SystemLocator.FriendManager.OnFriendListUpdated += OnFriendListUpdated;
        OnFriendListUpdated();
    }
    private void OnDisable()
    {
        GameInstaller.Instance.SystemLocator.FriendManager.OnFriendListUpdated -= OnFriendListUpdated;
        foreach (var friendTab in _usingFriendsTab)
        {
            friendTab.Value.gameObject.SetActive(false);
            _unusedFriendTabs.Add(friendTab.Value);
        }
        _usingFriendsTab.Clear();
    }

    #region UIElements_Factory
    
    private void AddFriendTab(FriendInfoModel friendInfoModel)
    {
        FriendsTab friendTab;
        if (_unusedFriendTabs.Count > 0)
        {
            friendTab = _unusedFriendTabs[0];
            _unusedFriendTabs.RemoveAt(0);
        }
        else
        {
            friendTab = Instantiate( transform.GetChild(0), transform).GetComponent<FriendsTab>();
        }
        friendTab.gameObject.SetActive(true);
        friendTab.SetData(friendInfoModel);
        _usingFriendsTab.Add(friendInfoModel.PlatformId, friendTab);
    }
    private void RemoveFriendTab(string platformId)
    {
        if (!_usingFriendsTab.ContainsKey(platformId)) return;
        _usingFriendsTab[platformId].gameObject.SetActive(false);
        _unusedFriendTabs.Add(_usingFriendsTab[platformId]);
        _usingFriendsTab.Remove(platformId);
    }
    #endregion
    
    private async void OnFriendListUpdated()
    {
        _friendModels = await GameInstaller.Instance.SystemLocator.FriendManager.GetFriendsList(friendshipStatus);
        
        foreach (var friend in _friendModels.Where(friend => !_usingFriendsTab.ContainsKey(friend.PlatformId)))
        {
            AddFriendTab(friend);
        }
        var removeList = _usingFriendsTab.Keys.Where(friend => _friendModels.All(x => x.PlatformId != friend)).ToList();
        foreach (var friend in removeList)
        {
            RemoveFriendTab(friend);
        }
    }
}
