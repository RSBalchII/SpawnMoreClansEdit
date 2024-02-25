using SpawnMoreClans.SaveData;
using TaleWorlds.CampaignSystem;

public SpawnMoreClansBehavior()
{
    _smcData = new SMCData();
    ((CampaignBehaviorBase)this)..ctor();
}
