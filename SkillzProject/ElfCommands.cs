﻿using ElfKingdom;

namespace MyBot
{
    class ElfCommands : StrategicCalculations
    {
        enum Building { Portal, Fountain };

        public override void DoTurn(Game game)
        {
            Elf[] myElves = game.GetMyLivingElves();
            Portal[] portals = game.GetMyPortals();
            Elf[] enemyElves = game.GetEnemyLivingElves();
            game.Debug("DDBETV: " + game.GetVolcano().DamageByEnemy);
            //If we have no elves, do nothing
            if (myElves.Length < 1)
            {
                return;
            }
            //Specific bot strategies
            SpecificBotStrategies(game, myElves);
            //End specific bot strategies
            /*
             * Life threatening danger:
             * If we have less than half our life, PPANIC!!!!!!! (HGTTG)
             */
            if (game.GetMyCastle().CurrentHealth <= game.GetMyCastle().MaxHealth / 2)
            {
                DefendAgainst(game.GetEnemyPortals(), game, myElves, EnemyVeryAggressivePortalRangeFromCastle, EnemyVeryAggressivePortalRangeFromElf);
            }
            /*
             * Kill straggelers:
             * Kill low health elves.
             */
            Elf[] lowHealthElves = System.Array.FindAll(enemyElves, elf => { game.Debug("Elf has " + elf.CurrentHealth + " health, kill? " + (elf.CurrentHealth < elf.MaxHealth / 6)); return elf.CurrentHealth < elf.MaxHealth / 6; });
            if (lowHealthElves.Length > 0)
            {
                DefendAgainst(lowHealthElves, game, myElves, 0, lowHealthElves[0].AttackRange * 2);
                game.Debug("Attacked elf!");
            }
            /*
             * Build fountains:
             * If we have less mana than the desired mana per turn, the offensive elf will build mana fountains
             * while the builder builds portals. 
             */
            if (game.GetMyself().ManaPerTurn <= DesiredManaPerTurn && !(game.GetMyself().ManaPerTurn == 0 && game.GetMyMana() < game.ManaFountainCost))
            {
                if (myElves[myElves.Length - 1].CanBuildManaFountain() && !myElves[myElves.Length - 1].AlreadyActed)
                {
                    AllocatedMana = 0;
                    myElves[myElves.Length - 1].BuildManaFountain();
                }
                else
                {
                    BuildInRadius(MinFountainBuildRadius, myElves[myElves.Length - 1], game, 2);
                }
            }
            /*
             * Attack volcano:
             * If we have a few portals, attack volcano.
             */
            if (game.GetVolcano().IsActive())
            {
                if (game.Turn < game.VolcanoInactiveTurns)
                {
                    DefendAgainst(new GameObject[] { game.GetVolcano() }, game, myElves, 9999, 0);
                }
                else
                {
                    DefendAgainst(new GameObject[] { game.GetVolcano() }, game, myElves, 0, game.GetMyPortals().Length * game.ElfAttackRange);
                }
            }
            /*
             * Build portals:
             * Our first priorety is to reach the desired portal amount. Until we do, building portals is our highest
             * priority. Once we do, defending becomes our highest priority. If we have less manaPerTurn than our ideal
             * mana per turn, we build more mana fountains, and then more portals.
             */
            {
                if (portals.Length >= DesiredPortalAmount)
                {
                    DefendPortalsAndElves(game, myElves);
                }
                else
                {
                    if (game.GetMyMana() > MinManaForPortal)
                    {
                        if (CanBuild(myElves[0], game))
                        {
                            AllocatedMana = 0;
                            myElves[0].BuildPortal();
                        }
                        else
                        {
                            game.Debug("0 Can't build. DI: " + myElves[0].Distance(game.GetMyCastle()) + " < " + MinPortalBuildRadius);
                            BuildInRadius(MinPortalBuildRadius, myElves[0], game);
                        }
                    }
                }
                if (myElves.Length >= 1 && !myElves[0].AlreadyActed)
                {
                    if (game.GetMyMana() > MinManaForPortal)
                    {
                        AllocatedMana = MinManaForPortal;
                        if (PortalsInRadius(MinPortalBuildRadius, game, BadMarginOfError) < DesiredPortalAmount)
                        {
                            if (CanBuild(myElves[0], game))
                            {
                                AllocatedMana = 0;
                                myElves[0].BuildPortal();
                            }
                            else
                            {
                                BuildInRadius(MinPortalBuildRadius, myElves[0], game);
                            }
                        }
                        else if (game.GetMyself().ManaPerTurn <= IdealManaPerTurn)
                        {
                            if (CanBuild(myElves[0], game, Building.Fountain))
                            {
                                AllocatedMana = 0;
                                myElves[0].BuildManaFountain();
                            }
                            else
                            {
                                BuildInRadius(MinFountainBuildRadius, myElves[0], game, 1);
                            }
                        }
                        else
                        {
                            if (CanBuild(myElves[0], game))
                            {
                                AllocatedMana = 0;
                                myElves[0].BuildPortal();
                            }
                            else
                            {
                                BuildInRadius(MinPortalBuildRadius, myElves[0], game);
                            }
                        }
                    }
                }
            }
            /*
             * If we have enough mana, we hide our elves from enemies (close elves/ice trolls).
             */
            if (game.GetMyMana() > 300) //Why 300?
            {
                foreach (var elf in myElves)
                {
                    //Hide oneself
                    Hide(elf, game);
                }
            }
            /*
             * Defense:
             * Our next priority is to defend against everything: first close elves, then close portals, then far elves
             * , then far portals, then mana fountains and lastly lava giants (since trolls can pretty easily deal with
             * those).
             */
            DefendAgainst(game.GetEnemyManaFountains(), game, myElves, 0, game.ElfAttackRange);
            DefendPortalsAndElves(game, myElves);
            DefendAgainst(game.GetEnemyManaFountains(), game, myElves, EnemyAggressivePortalRangeFromCastle, EnemyAggressivePortalRangeFromElf);
            DefendAgainst(game.GetEnemyLavaGiants(), game, myElves, EnemyAggressiveLavaGiantRangeFromCastle, EnemyAggressiveLavaGiantRangeFromElf);
            /*
             * Next, we defend our portals from elves near them.
             */
            DefendOn(game.GetMyPortals(), game.GetEnemyLivingElves(), myElves, EnemyAggressiveElfRangeFromPortal);
            DefendOn(game.GetMyPortals(), game.GetEnemyTornadoes(), myElves, EnemyAggressiveTornadoRangeFromPortal);
            /*
             * Elf 0, our builder, will then proceed to try and build more portals (in case we currently don't have
             * enough mana, our elves will prioritize looking at enemies over building, but our builder shouldn't).
             */
            if (myElves.Length > 1)
            {
                BuildInRadius(MinPortalBuildRadius, myElves[0], game);
            }
            /*
             * Look at enemies:
             * Our idle elves will look at enemies to be ready to intercept them once they get too close.
             */
            if (game.GetEnemyLivingElves().Length > 0)
            {
                for (int i = 1; i < myElves.Length; i++)
                {
                    if (myElves[i].AlreadyActed)
                    {
                        continue;
                    }
                    Elf nearestElf = game.GetEnemyLivingElves()[0];
                    foreach (var item in game.GetEnemyLivingElves())
                    {
                        if (item.Distance(myElves[i]) < nearestElf.Distance(myElves[i]))
                        {
                            nearestElf = item;
                        }
                    }

                    myElves[i].MoveTo(Cis(DefendRadius, BaseDegree, game.GetMyCastle().Location));
                }
            }
            /*
             * Pretend you're working:
             * In case all enemy elves are dead, we can "pass the time" by building more portals.
             */
            foreach (var elf in myElves)
            {
                if (elf.AlreadyActed)
                {
                    continue;
                }
                if (game.GetMyMana() > MinManaForPortal && PortalsInRadius(MinPortalBuildRadius, game) < DesiredPortalAmount && elf.CanBuildPortal())
                {
                    elf.BuildPortal();
                    continue;
                }
                BuildInRadius(MinPortalBuildRadius, elf, game);
            }
        }
        /// <summary>
        /// Defends against specific bots.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="portals"></param>
        void SpecificBotStrategies(Game game, Elf[] myElves)
        {
            try
            {
                //Z...
                if (game.GetEnemyTornadoes()[0].SuffocationPerTurn == 0)
                {
                    //ZHell = true;
                    /*game.Debug("Charge!");
                    foreach (Elf elf in myElves)
                    {
                        if (elf.InAttackRange(game.GetEnemyCastle()))
                        {
                            game.Debug("Attacked");
                            elf.Attack(game.GetEnemyCastle());
                        }
                        else if (elf.CanCastSpeedUp())
                        {
                            game.Debug("Charged");
                            elf.CastSpeedUp();
                        }
                        else if (elf.CanCastInvisibility())
                        {
                            game.Debug("Hid in plain sight");
                            elf.CastInvisibility();
                        }
                        else
                        {
                            game.Debug("Walked");
                            elf.MoveTo(game.GetEnemyCastle());
                        }
                    }*/
                    if (game.GetMyManaFountains().Length < 2)
                    {
                        Elf[] firstElf = new Elf[1];
                        firstElf[0] = myElves[0];
                        DefendAgainst(game.GetEnemyTornadoes(), game, firstElf, 9999, 9999);
                    }
                    else
                    {
                        DefendAgainst(game.GetEnemyTornadoes(), game, myElves, 9999, 9999, false);
                    }
                }
            }
            catch { }
            if (game.CastleMaxHealth <= 2)
            {
                //Labyrinth...
                foreach (var elf in myElves)
                {
                    if (elf.CurrentSpells.Length == 0 && elf.CanCastInvisibility())
                    {
                        elf.CastInvisibility();
                        continue;
                    }
                    if (elf.InAttackRange(game.GetEnemyCastle()))
                    {
                        elf.Attack(game.GetEnemyCastle());
                    }
                    else
                    {
                        elf.MoveTo(game.GetEnemyCastle());
                    }
                }
            }
            if (game.PortalCost >= 10000)
            {
                //HoraMana...
                foreach (var elf in myElves)
                {
                    if (elf.CurrentSpells.Length == 0 && elf.CanCastInvisibility())
                    {
                        elf.CastInvisibility();
                        continue;
                    }
                    if (elf.InAttackRange(game.GetEnemyCastle()))
                    {
                        elf.Attack(game.GetEnemyCastle());
                    }
                    else
                    {
                        elf.MoveTo(game.GetEnemyCastle());
                    }
                }
            }
            if (game.ElfMaxSpeed == 0)
            {
                //Trap...
                foreach (var elf in myElves)
                {
                    if (elf.CurrentSpells.Length == 0 && elf.CanCastInvisibility() && !elf.AlreadyActed)
                    {
                        IceTroll[] iceTrolls = game.GetEnemyIceTrolls();
                        foreach (var item in iceTrolls)
                        {
                            if (item.Distance(elf) <= game.IceTrollAttackRange)
                            {
                                elf.CastInvisibility();
                                break;
                            }
                        }
                    }
                }
            }
            if (game.ElfMaxSpeed <= 5)
            {
                //IGotStamana...
                foreach (var elf in myElves)
                {
                    if (elf.CurrentSpells.Length == 0 && elf.CanCastSpeedUp() && game.GetMyMana() > game.ManaFountainCost && !elf.AlreadyActed)
                    {
                        elf.CastSpeedUp();
                    }
                }
            }
            if (game.DefaultManaPerTurn <= 0)
            {
                DefendAgainst(game.GetEnemyManaFountains(), game, myElves, EnemyAggressivePortalRangeFromCastle, EnemyAggressivePortalRangeFromElf);
            }
            /*if (ZHell && game.GetMyCastle().CurrentHealth < 40 && game.GetEnemyLivingElves().Length == 0)
            {
                DefendAgainst(game.GetEnemyManaFountains(), game, myElves, 9999, 9999);
                DefendAgainst(game.GetEnemyPortals(), game, myElves, 9999, 9999);
                ChanceForGlory(game, myElves);
            }*/
        }
        /// <summary>
        /// Runs defendAgainst against portals and elves using our normal consts.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="myElves"></param>
        public void DefendPortalsAndElves(Game game, Elf[] myElves)
        {
            GameObject[] enemyElves = game.GetEnemyLivingElves();
            GameObject[] enemyPortals = game.GetEnemyPortals();
            DefendAgainst(enemyPortals, game, myElves, EnemyVeryAggressivePortalRangeFromCastle, EnemyVeryAggressivePortalRangeFromElf);
            DefendAgainst(enemyElves, game, myElves, EnemyAggressiveElfRangeFromCastle, EnemyAggressiveElfRangeFromElf);
            DefendAgainst(enemyPortals, game, myElves, EnemyAggressivePortalRangeFromCastle, EnemyAggressivePortalRangeFromElf);
        }
        /// <summary>
        /// Finds the best (nearest) elf to attack each dangerous target (closer to our castle than range or to our elves than elfRange).
        /// </summary>
        /// <param name="targets"></param>
        /// <param name="game"></param>
        /// <param name="myElves"></param>
        /// <param name="range"></param>
        /// <param name="elfRange"></param>
        public void DefendAgainst(GameObject[] targets, Game game, Elf[] myElves, int range = 1500, int elfRange = 0, bool canUseBoost = true, bool forceUseBoost = false)
        {
            GameObject[] defult = new GameObject[myElves.Length];
            int[] minDist = new int[myElves.Length];
            for (int i = 0; i < myElves.Length; i++)
            {
                minDist[i] = int.MaxValue;
            }
            for (int i = 0; i < myElves.Length; i++)
            {
                if (myElves[i] == null)
                {
                    continue;
                }
                foreach (var creature in targets)
                {
                    if (creature == null)
                    {
                        continue;
                    }
                    if (creature.GetLocation() == null)
                    {
                        continue;
                    }
                    if ((!true || creature.Distance(game.GetMyCastle()) <= MaxBuildRadius) && (creature.Distance(game.GetMyCastle()) <= range || creature.Distance(myElves[i]) <= elfRange))
                    {
                        if (false && System.Array.Exists(defult, element => element == creature))
                        {
                            game.Debug(creature + " is already a target");
                            /*if (creature.Distance(myElves[i]) + 100 < minDist[i])
                            {
                                game.Debug("Target " + i + " is now " + creature);
                                defult[i] = creature;
                                minDist[i] = creature.Distance(myElves[i]);
                            }*/
                        }
                        else if (creature.Distance(myElves[i]) < minDist[i])
                        {
                            game.Debug("Target " + i + " is now " + creature);
                            defult[i] = creature;
                            minDist[i] = creature.Distance(myElves[i]);
                        }
                    }
                    //Order elf to defend
                }
            }
            for (int i = 0; i < myElves.Length; i++)
            {
                if (myElves[i] == null)
                {
                    continue;
                }
                if (defult[i] == null)
                {
                    continue;
                }
                if (myElves[i].AlreadyActed)
                {
                    continue;
                }
                game.Debug("Elf " + myElves[i].Id + " attacked!");
                if (myElves[i].InAttackRange(defult[i]))
                {
                    myElves[i].Attack(defult[i]);
                }
                else
                {
                    if ((canUseBoost && game.GetMyMana() > myElves[i].Distance(defult[i]) / 3) || forceUseBoost)
                    {
                        if (myElves[i].CurrentSpells.Length == 0 && myElves[i].CanCastSpeedUp())
                        {
                            myElves[i].CastSpeedUp();
                        }
                    }
                    if (!myElves[i].AlreadyActed)
                    {
                        myElves[i].MoveTo(defult[i]);
                    }
                }
            }
        }
        /// <summary>
        /// Moves the builder to the best position in the desired radius for a portal/mana fountain. This function recursively finds the nearest possible radius where we can build a portal). This function MOVES the elf, but doesn't order it to BUILD.
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="builder"></param>
        /// <param name="game"></param>
        /// <param name="degree"></param>
        /// <param name="degreeModifier"></param>
        public void BuildInRadius(double radius, Elf builder, Game game, double degreeModifier = 2.0)
        {
            if (builder.AlreadyActed)
                return;
            //game.Debug("Wasted: " + ManaWasted);
            //game.Debug("Max potential: " + MaxPotentialMana);
            //game.Debug("Max radius: " + MaxBuildRadius);
            //game.Debug("Min radius: " + MinPortalBuildRadius);
            //game.Debug("Radius: " + radius);
            if (ManaWasted >= MaxPotentialMana / 2)
            {
                //game.Debug("Quit?");
                if (radius > MaxPotentialMana - ManaWasted && radius > MinPortalBuildRadius + game.PortalSize * 2)
                {
                    //game.Debug("Quit!");
                    return;
                }
            }
            if (radius > MaxBuildRadius)
            {
                game.Debug("Above max!");
                return;
            }
            Location baseLocation = game.GetMyCastle().Location;
            Location target;
            double modifier = PI / 8;
            // Do not delete this
            /*for (int i = 1; i <= Ceiling(5 / degreeModifier); i++)
            {
                double temp = (i / 2) * Pow(-1, i) * modifier;
                target = Cis(radius, BaseDegree + temp);
                target.Col += baseLocation.Col;
                target.Row += baseLocation.Row;
                if (game.CanBuildPortalAt(target))
                {
                    MoveToBuild(builder, game, target);
                }
            }*/
            modifier = PI / 40;
            for (int i = 1; i <= Ceiling(25 / degreeModifier); i++)
            {
                double temp = (i / 2) * Pow(-1, i) * modifier;
                target = new Location((int)(radius * Sin(BaseDegree + temp)), (int)(radius * Cos(BaseDegree + temp)));
                target.Col += baseLocation.Col;
                target.Row += baseLocation.Row;
                if (game.CanBuildPortalAt(target))
                {
                    MoveToBuild(builder, game, target);
                }
            }
            BuildInRadius(radius + game.PortalSize, builder, game);
        }
        /// <summary>
        /// A helper function for BuildInRadius.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="game"></param>
        /// <param name="target"></param>
        void MoveToBuild(Elf builder, Game game, Location target)
        {
            //Hide oneself
            game.Debug("Elf " + builder.Id + " tried to hide. Elf " + builder.AlreadyActed + " acted.");
            Hide(builder, game);
            game.Debug("Elf " + builder.Id + " failed? to hide. Elf " + builder.AlreadyActed + " acted.");
            if (builder.AlreadyActed)
                return;
            if (builder.Location == target)
                return;
            builder.MoveTo(target);
            game.Debug("Elf " + builder.Id + " moved to build. Elf " + builder.AlreadyActed + " acted.");
            return;
        }
        /// <summary>
        /// Checks whether the elf should hide and hides it.
        /// </summary>
        /// <param name="elf"></param>
        /// <param name="game"></param>
        /// <param name="requiredMana"></param>
        void Hide(Elf elf, Game game, int requiredMana = 100)
        {
            if (game.GetMyMana() <= requiredMana)
            {
                return;
            }
            if (!elf.AlreadyActed && elf.CurrentSpells.Length == 0 && elf.CanCastInvisibility())
            {
                IceTroll[] iceTrolls = game.GetEnemyIceTrolls();
                foreach (var item in iceTrolls)
                {
                    if (item.Distance(elf) <= game.IceTrollAttackRange)
                    {
                        game.Debug("Hiding");
                        elf.CastInvisibility();
                        return;
                    }
                }
                Elf[] enemyElves = game.GetEnemyLivingElves();
                foreach (var item in enemyElves)
                {
                    if (item.Distance(elf) <= game.ElfAttackRange)
                    {
                        game.Debug("Hiding");
                        elf.CastInvisibility();
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// Counts the amount of portals in a radius.
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="game"></param>
        /// <param name="marginOfError"></param>
        /// <returns></returns>
        int PortalsInRadius(int radius, Game game, int marginOfError = 150)
        {
            int portalCount = 0;
            Location castlePos = game.GetMyCastle().Location;
            foreach (Portal item in game.GetMyPortals())
            {
                if (item.Distance(castlePos) >= radius - marginOfError && item.Distance(castlePos) <= radius + marginOfError)
                {
                    portalCount++;
                }
            }
            return portalCount;
        }
        /// <summary>
        /// The opposite of defendAgainst: finds the nearest object in danger to protect and protect it.
        /// </summary>
        /// <param name="protectIn"></param>
        /// <param name="protectFrom"></param>
        /// <param name="myElves"></param>
        /// <param name="radiusToDefend"></param>
        public void DefendOn(GameObject[] protectIn, GameObject[] protectFrom, Elf[] myElves, int radiusToDefend)
        {
            if (protectIn.Length < 1 || protectFrom.Length < 1)
            {
                return;
            }
            if (myElves.Length > 0)
            {
                foreach (var elf in myElves)
                {
                    if (elf.AlreadyActed)
                    {
                        continue;
                    }
                    int minDistance = radiusToDefend;
                    GameObject attack = null;
                    foreach (var build in protectIn)
                    {
                        foreach (var enemy in protectFrom)
                        {
                            if (radiusToDefend >= build.Location.Distance(enemy.Location))
                            {
                                if (minDistance >= build.Location.Distance(enemy.Location))
                                {
                                    minDistance = build.Location.Distance(enemy.Location);
                                    attack = enemy;
                                }
                            }
                        }
                    }
                    if (attack == null)
                    {
                        continue;
                    }
                    if (elf.InAttackRange(attack))
                    {
                        elf.Attack(attack);
                    }
                    else
                    {
                        elf.MoveTo(attack);
                    }
                }
            }
        }
        /// <summary>
        /// Checks if the elf can build (more complicated than it sounds).
        /// </summary>
        /// <param name="elf"></param>
        /// <param name="game"></param>
        /// <param name="building"></param>
        /// <returns></returns>
        bool CanBuild(Elf elf, Game game, Building building = Building.Portal)
        {
            switch (building)
            {
                case Building.Portal:
                    if (!elf.CanBuildPortal() || elf.AlreadyActed || elf.Distance(game.GetMyCastle()) < MinPortalBuildRadius - 2)
                        return false;
                    break;
                case Building.Fountain:
                    if (!elf.CanBuildManaFountain() || elf.AlreadyActed)
                        return false;
                    break;
                default:
                    break;
            }
            foreach (Elf anElf in game.GetMyLivingElves())
            {
                if (anElf == elf)
                    continue;
                if (anElf.Distance(elf) <= game.PortalSize * 2 && anElf.IsBuilding)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Chrages thoughtlessly into the enemy.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="myElves"></param>
        void ChanceForGlory(Game game, Elf[] myElves)
        {
            game.Debug("Charge!");
            foreach (Elf elf in myElves)
            {
                if (elf == null)
                {
                    continue;
                }
                if (elf.AlreadyActed)
                {
                    continue;
                }
                if (elf.InAttackRange(game.GetEnemyCastle()))
                {
                    game.Debug("Attacked");
                    elf.Attack(game.GetEnemyCastle());
                }
                else if (elf.CanCastSpeedUp())
                {
                    game.Debug("Charged");
                    elf.CastSpeedUp();
                }
                else if (elf.CanCastInvisibility())
                {
                    game.Debug("Hid in plain sight");
                    elf.CastInvisibility();
                }
                else
                {
                    game.Debug("Walked");
                    elf.MoveTo(game.GetEnemyCastle());
                }
            }
        }
    }
}
