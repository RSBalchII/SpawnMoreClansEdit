using TaleWorlds.CampaignSystem;

public void OnSessionLaunched(CampaignGameStarter gameStarter)
{
    //IL_002a: Unknown result type (might be due to invalid IL or missing references)
    //IL_002f: Unknown result type (might be due to invalid IL or missing references)
    if (((CampaignTime)(ref _smcData.LastSpawned)).ToDays <= 0.0)
    {
        _smcData.LastSpawned = CampaignTime.Now;
    }
}
