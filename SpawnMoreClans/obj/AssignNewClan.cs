using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Helpers;
using MCM.Abstractions.Base.Global;
using SpawnMoreClans.Helper;
using SpawnMoreClans.NameGeneration;
using SpawnMoreClans.SaveData;
using SpawnMoreClans.Settings;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace SpawnMoreClans.Behavior;

internal class SpawnMoreClansBehavior : CampaignBehaviorBase
{
    public SMCData _smcData = new SMCData();

    public override void RegisterEvents()
    {
        CampaignEvents.DailyTickEvent.AddNonSerializedListener((object)this, (Action)OnDailyTick);
        CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener((object)this, (Action<CampaignGameStarter>)OnSessionLaunched);
    }

    public override void SyncData(IDataStore dataStore)
    {
        dataStore.SyncData<SMCData>("SpawnMoreClans_Data", ref _smcData);
    }

    public void OnSessionLaunched(CampaignGameStarter gameStarter)
    {
        //IL_002a: Unknown result type (might be due to invalid IL or missing references)
        //IL_002f: Unknown result type (might be due to invalid IL or missing references)
        if (((CampaignTime)(ref _smcData.LastSpawned)).ToDays <= 0.0)
        {
            _smcData.LastSpawned = CampaignTime.Now;
        }
    }

    public void OnDailyTick()
    {
        //IL_0012: Unknown result type (might be due to invalid IL or missing references)
        //IL_0017: Unknown result type (might be due to invalid IL or missing references)
        if (CanSpawnClans())
        {
            _smcData.LastSpawned = CampaignTime.Now;
            for (int i = 0; i < GlobalSettings<MCMSettings>.Instance.NumberOfClansToSpawnAtOnce; i++)
            {
                AttemptToGenerateNewClan();
            }
        }
    }

    public void AttemptToGenerateNewClan()
    {
        //IL_0066: Unknown result type (might be due to invalid IL or missing references)
        //IL_0070: Expected O, but got Unknown
        //IL_02c8: Unknown result type (might be due to invalid IL or missing references)
        //IL_02cd: Unknown result type (might be due to invalid IL or missing references)
        //IL_00fa: Unknown result type (might be due to invalid IL or missing references)
        //IL_0106: Unknown result type (might be due to invalid IL or missing references)
        //IL_0124: Unknown result type (might be due to invalid IL or missing references)
        //IL_012f: Expected O, but got Unknown
        //IL_012f: Expected O, but got Unknown
        //IL_02f9: Unknown result type (might be due to invalid IL or missing references)
        //IL_020b: Unknown result type (might be due to invalid IL or missing references)
        //IL_0210: Unknown result type (might be due to invalid IL or missing references)
        //IL_04ec: Unknown result type (might be due to invalid IL or missing references)
        //IL_04f3: Expected O, but got Unknown
        //IL_04f9: Unknown result type (might be due to invalid IL or missing references)
        //IL_0500: Expected O, but got Unknown
        //IL_0506: Unknown result type (might be due to invalid IL or missing references)
        //IL_050d: Expected O, but got Unknown
        //IL_0513: Unknown result type (might be due to invalid IL or missing references)
        //IL_051a: Expected O, but got Unknown
        //IL_055d: Unknown result type (might be due to invalid IL or missing references)
        //IL_0569: Expected O, but got Unknown
        //IL_0433: Unknown result type (might be due to invalid IL or missing references)
        //IL_0438: Unknown result type (might be due to invalid IL or missing references)
        //IL_043b: Unknown result type (might be due to invalid IL or missing references)
        //IL_045a: Unknown result type (might be due to invalid IL or missing references)
        //IL_023c: Unknown result type (might be due to invalid IL or missing references)
        //IL_0279: Unknown result type (might be due to invalid IL or missing references)
        Kingdom randomKingdom = SMCHelper.GetRandomKingdom();
        if (randomKingdom == null)
        {
            return;
        }
        Random random = new Random();
        bool flag = random.Next(0, 101) > GlobalSettings<MCMSettings>.Instance.ChanceOfCreatedClanJoiningKingdoms;
        string newClanName = "";
        Clan val = null;
        int num = 0;
        do
        {
            if (num > 1000)
            {
                InformationManager.DisplayMessage(new InformationMessage("[SpawnMoreClans]: Error spawning new clan. SpawnMoreClans is unable to generate a new clan name that is unused! There are too many clans!"));
                return;
            }
            newClanName = ClanNameGenerator.Instance.GenerateRandomName();
            val = Clan.FindFirst((Predicate<Clan>)((Clan x) => ((object)x.Name).ToString() == newClanName));
            num++;
        }
        while (val != null);
        CultureObject randomCultureObject = SMCHelper.GetRandomCultureObject();
        Clan newClan = Clan.CreateClan("SMC_" + newClanName.ToLower());
        Settlement val2 = SMCHelper.GetRandomSettlementOfCulture(randomCultureObject);
        if (val2 == null)
        {
            val2 = SMCHelper.GetRandomSettlement();
        }
        newClan.InitializeClan(new TextObject(newClanName, (Dictionary<string, object>)null), new TextObject(newClanName, (Dictionary<string, object>)null), randomCultureObject, Banner.CreateRandomClanBanner(Extensions.GetDeterministicHashCode(((MBObjectBase)newClan).StringId)), val2.GatePosition, true);
        newClan.UpdateHomeSettlement(val2);
        int randomTierLevel = SMCHelper.GetRandomTierLevel();
        while (newClan.Tier < randomTierLevel)
        {
            Clan obj = newClan;
            obj.Renown += 1f;
            int num2 = Campaign.Current.Models.ClanTierModel.CalculateTier(newClan);
            if (num2 >= randomTierLevel)
            {
                ((object)newClan).GetType().GetField("_tier", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(newClan, num2);
                ((CampaignEventReceiver)CampaignEventDispatcher.Instance).OnClanTierChanged(newClan, true);
                break;
            }
        }
        Hero val3 = SMCHelper.GenerateRandomHero(randomCultureObject, val2);
        val3.IsMinorFactionHero = flag;
        newClan.SetLeader(val3);
        EquipmentElement bannerItem = val3.BannerItem;
        if (((EquipmentElement)(ref bannerItem)).IsInvalid())
        {
            ItemObject randomBannerItemForHero = BannerHelper.GetRandomBannerItemForHero(val3);
            if (randomBannerItemForHero != null)
            {
                val3.BannerItem = new EquipmentElement(randomBannerItemForHero, (ItemModifier)null, (ItemObject)null, false);
            }
        }
        val3.ChangeState((CharacterStates)(Campaign.Current.GameStarted ? 1 : 0));
        newClan.CreateNewMobileParty(val3);
        try
        {
            val3.PartyBelongedTo.Position2D = val2.GatePosition;
        }
        catch
        {
        }
        GiveGoldAction.ApplyBetweenCharacters((Hero)null, val3, 20000, false);
        for (int i = 1; i < Campaign.Current.Models.MinorFactionsModel.MinorFactionHeroLimit; i++)
        {
            Hero val4 = SMCHelper.GenerateRandomHero(randomCultureObject, val2);
            val4.IsMinorFactionHero = flag;
            val4.Clan = newClan;
            bannerItem = val4.BannerItem;
            if (((EquipmentElement)(ref bannerItem)).IsInvalid())
            {
                ItemObject randomBannerItemForHero2 = BannerHelper.GetRandomBannerItemForHero(val4);
                if (randomBannerItemForHero2 != null)
                {
                    val4.BannerItem = new EquipmentElement(randomBannerItemForHero2, (ItemModifier)null, (ItemObject)null, false);
                }
            }
            newClan.CreateNewMobileParty(val4);
            val4.ChangeState((CharacterStates)(Campaign.Current.GameStarted ? 1 : 0));
        }
        if (flag)
        {
            ((object)newClan).GetType().GetProperty("IsMinorFaction", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(newClan, true);
            ((object)newClan).GetType().GetProperty("IsClanTypeMercenary", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(newClan, true);
            ((object)newClan).GetType().GetField("_defaultPartyTemplate", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(newClan, randomCultureObject.DefaultPartyTemplate);
            ((object)newClan).GetType().GetField("_minorFactionCharacterTemplates", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(newClan, ((IEnumerable<CharacterObject>)randomCultureObject.LordTemplates).ToList());
            CharacterObject basicTroop = null;
            PartyTemplateObject defaultPartyTemplate = newClan.DefaultPartyTemplate;
            int num3 = 50;
            foreach (PartyTemplateStack item in (List<PartyTemplateStack>)(object)defaultPartyTemplate.Stacks)
            {
                int level = ((BasicCharacterObject)item.Character).Level;
                if (level < num3)
                {
                    num3 = level;
                    basicTroop = item.Character;
                }
            }
            newClan.BasicTroop = basicTroop;
        }
        else
        {
            newClan.IsNoble = true;
            if (random.Next(0, 101) <= GlobalSettings<MCMSettings>.Instance.ChanceOfJoiningPlayerKingdom && Clan.PlayerClan.Kingdom != null && Clan.PlayerClan.Kingdom.Leader == Hero.MainHero)
            {
                TextObject val5 = new TextObject("{=GameMenu_SMC_Title}A messenger from a new rising clan \"{CLAN_NAME}\" arrives.", (Dictionary<string, object>)null);
                TextObject val6 = new TextObject("{=GameMenu_SMC_Desc}The messenger says that the clan leader is proposing for {CLAN_NAME} to join your kingdom! Do you accept his proposal?", (Dictionary<string, object>)null);
                TextObject val7 = new TextObject("{=GameMenu_SMC_Yes}Yes", (Dictionary<string, object>)null);
                TextObject val8 = new TextObject("{=GameMenu_SMC_No}No", (Dictionary<string, object>)null);
                InformationManager.ShowInquiry(new InquiryData(((object)val5).ToString(), ((object)val6).ToString(), true, true, ((object)val7).ToString(), ((object)val8).ToString(), (Action)delegate
                {
                    AssignNewClan(newClan, joinedPlayerKingdom: true, isMinorClan: false);
                }, (Action)delegate
                {
                    AssignNewClan(newClan, joinedPlayerKingdom: false, isMinorClan: false);
                }, "", 0f, (Action)null, (Func<ValueTuple<bool, string>>)null, (Func<ValueTuple<bool, string>>)null), true, true);
            }
        }
        AssignNewClan(newClan, joinedPlayerKingdom: false, flag);
    }

    public void AssignNewClan(Clan newClan, bool joinedPlayerKingdom, bool isMinorClan)
    {
        //IL_009f: Unknown result type (might be due to invalid IL or missing references)
        //IL_00a5: Expected O, but got Unknown
        //IL_00d4: Unknown result type (might be due to invalid IL or missing references)
        //IL_00de: Expected O, but got Unknown
        if (joinedPlayerKingdom)
        {
            newClan.Kingdom = Clan.PlayerClan.Kingdom;
            ((object)newClan).GetType().GetMethod("UpdateBannerColorsAccordingToKingdom", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(newClan, null);
        }
        else if (!isMinorClan)
        {
            Kingdom val = SMCHelper.GetRandomKingdomOfCulture(newClan.Culture);
            if (val == null)
            {
                val = SMCHelper.GetRandomKingdom();
            }
            ChangeKingdomAction.ApplyByJoinToKingdom(newClan, val, true);
            ((object)newClan).GetType().GetMethod("UpdateBannerColorsAccordingToKingdom", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(newClan, null);
        }
        else if (!isMinorClan)
        {
        }
        Campaign.Current.EncyclopediaManager.CreateEncyclopediaPages();
        TextObject val2 = new TextObject("{=Announcement_SMC}A new clan, {CLAN_NAME} has arisen around {SETTLEMENT_NAME}!", (Dictionary<string, object>)null);
        val2.SetTextVariable("CLAN_NAME", newClan.Name);
        val2.SetTextVariable("SETTLEMENT_NAME", newClan.HomeSettlement.Name);
        InformationManager.DisplayMessage(new InformationMessage(((object)val2).ToString()));
    }

    public bool CanSpawnClans()
    {
        if (_smcData == null)
        {
            _smcData = new SMCData();
        }
        return (int)((CampaignTime)(ref _smcData.LastSpawned)).ElapsedDaysUntilNow >= GlobalSettings<MCMSettings>.Instance.DaysBeforeNewClansCanSpawn;
    }
}
