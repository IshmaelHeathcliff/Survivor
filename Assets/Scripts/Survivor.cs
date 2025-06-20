﻿using Core;
using Core.Scene;
using GamePlay.Character.Enemy;
using GamePlay.Character.State;
using GamePlay.Character.Modifier;
using GamePlay.Character.Player;
using GamePlay.Character.Damage;
using Data.SaveLoad;
using GamePlay.Item;
using GamePlay.Skill;
using GamePlay.Character;

public class GameFrame : Architecture<GameFrame>
{
    protected override void Init()
    {
        RegisterModel(new PlayersModel());
        RegisterModel(new EnemiesModel());
        RegisterModel(new SceneModel());

        RegisterSystem(new InputSystem());
        RegisterSystem(new ModifierSystem());
        RegisterSystem(new StateCreateSystem());
        RegisterSystem(new DropSystem());
        RegisterSystem(new SkillSystem());
        RegisterSystem(new SkillGachaSystem()); ;
        RegisterSystem(new SkillReleaseSystem());
        RegisterSystem(new ResourceSystem());
        RegisterSystem(new CountSystem());
        RegisterSystem(new AttackerCreateSystem());
        RegisterSystem(new PositionQuerySystem());

        RegisterUtility(new SaveLoadUtility());
    }
}
