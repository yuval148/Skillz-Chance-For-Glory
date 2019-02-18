using ElfKingdom;

namespace MyBot
{
    class ElfCommands : StrategicCalculations
    {
        enum Building { Portal, Fountain };
        //To do: after trun 600, elves start attacking
        public override void DoTurn(Game game)
        {
            Elf[] myElves = game.GetMyLivingElves();
            Portal[] portals = game.GetMyPortals();
            if (myElves.Length < 1)
            {
                return;
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
            if (game.GetMyself().ManaPerTurn <= DesiredManaPerTurn && !(game.GetMyself().ManaPerTurn == 0 && game.GetMyMana() < game.ManaFountainCost))
            {
                if (myElves[myElves.Length - 1].CanBuildManaFountain() && !myElves[myElves.Length - 1].AlreadyActed)
                {
                    myElves[myElves.Length - 1].BuildManaFountain();
                }
                else
                {
                    BuildInRadius(MinFountainBuildRadius, myElves[myElves.Length - 1], game, 1);
                }
            }
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
                            myElves[0].BuildPortal();
                        }
                        else
                        {
                            BuildInRadius(MinPortalBuildRadius, myElves[0], game);
                        }
                    }
                }
                if (myElves.Length >= 1 && !myElves[0].AlreadyActed)
                {
                    if (game.GetMyMana() > MinManaForPortal)
                    {
                        if (PortalsInRadius(MinPortalBuildRadius, game, 1500) < DesiredPortalAmount)
                        {
                            if (CanBuild(myElves[0], game))
                            {
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
            //Hide oneself
            if (game.GetMyMana() > 300)
            {
                foreach (var elf in myElves)
                {
                    //Hide oneself
                    Hide(elf, game);
                }
            }
            DefendAgainst(game.GetEnemyManaFountains(), game, myElves, 0, game.ElfAttackRange);
            DefendPortalsAndElves(game, myElves);
            DefendAgainst(game.GetEnemyManaFountains(), game, myElves, EnemyAggressivePortalRangeFromCastle, EnemyAggressivePortalRangeFromElf);
            DefendAgainst(game.GetEnemyLavaGiants(), game, myElves, EnemyAggressiveLavaGiantRangeFromCastle, EnemyAggressiveLavaGiantRangeFromElf);
            //Defult 1 - defend portals
            DefendOn(game.GetMyPortals(), game.GetEnemyLivingElves(), myElves, EnemyAggressiveElfRangeFromPortal);
            //Defult 2 - look at enemies
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
                    double degree = DegreeBetween(nearestElf.Location, game.GetMyCastle().Location);
                    myElves[i].MoveTo(Cis(DefendRadius, degree, game.GetMyCastle().Location));
                }
            }
            //Defult 3 - pretend you're working
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
        public void DefendPortalsAndElves(Game game, Elf[] myElves)
        {
            GameObject[] enemyElves = game.GetEnemyLivingElves();
            GameObject[] enemyPortals = game.GetEnemyPortals();
            DefendAgainst(enemyPortals, game, myElves, (int)(EnemyAggressivePortalRangeFromCastle / 1.5), (int)(EnemyAggressivePortalRangeFromElf / 1.5));
            DefendAgainst(enemyElves, game, myElves, EnemyAggressiveElfRangeFromCastle, EnemyAggressiveElfRangeFromElf);
            DefendAgainst(enemyPortals, game, myElves, EnemyAggressivePortalRangeFromCastle, EnemyAggressivePortalRangeFromElf);
        }
        public void DefendAgainst(GameObject[] arrayOfType, Game game, Elf[] myElves, int range = 1500, int elfRange = 0)
        {
            GameObject[] defult = new GameObject[myElves.Length];
            int[] minDist = new int[myElves.Length];
            for (int i = 0; i < myElves.Length; i++)
            {
                minDist[i] = int.MaxValue;
            }
            foreach (var creature in arrayOfType)
            {
                if (creature == null)
                {
                    continue;
                }
                if (creature.GetLocation() == null)
                {
                    continue;
                }
                for (int i = 0; i < myElves.Length; i++)
                {
                    if (creature.Distance(game.GetMyCastle()) <= range || creature.Distance(myElves[i]) <= elfRange)
                    {
                        if (myElves[i] == null)
                        {
                            continue;
                        }
                        if (creature.Distance(myElves[i]) < minDist[i])
                        {
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
                if (myElves[i].InAttackRange(defult[i]))
                {
                    myElves[i].Attack(defult[i]);
                }
                else
                {
                    if (game.GetMyMana() > myElves[i].Distance(defult[i]) / 3)
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
        public void BuildInRadius(double radius, Elf builder, Game game, double degree = PI / 3, double degreeModifier = 2.0)
        {
            if (builder.AlreadyActed)
                return;
            game.Debug("Wasted: " + ManaWasted);
            game.Debug("Max potential: " + MaxPotentialMana);
            game.Debug("Max radius: " + MaxBuildRadius);
            game.Debug("Min radius: " + MinPortalBuildRadius);
            game.Debug("Radius: " + radius);
            if (ManaWasted >= MaxPotentialMana / 2)
            {
                game.Debug("Quit?");
                if (radius > MaxPotentialMana - ManaWasted && radius > MinPortalBuildRadius + 300)
                {
                    game.Debug("Quit!");
                    return;
                }
            }
            if (radius > MaxBuildRadius)
            {
                game.Debug("Above max!");
                return;
            }
            Location baseLocation = game.GetMyCastle().Location;
            double baseDegree;
            double a = Sqrt(Pow(baseLocation.Col - game.GetEnemyCastle().Location.Col, 2) + Pow(baseLocation.Row - game.GetEnemyCastle().Location.Row, 2));
            double b = Abs(baseLocation.Col - game.GetEnemyCastle().Location.Col);
            baseDegree = Asin(b / a);
            baseDegree = PI / 4;
            int min = int.MaxValue;
            for (int i = 0; i < 8; i++)
            {
                if (game.GetEnemyCastle().Distance(Cis(100, PI / 4 + PI * i / 4, game.GetMyCastle().Location)) < min)
                {
                    min = game.GetEnemyCastle().Distance(Cis(100, PI / 4 + PI * i / 4, game.GetMyCastle().Location));
                    baseDegree = PI / 4 + PI * i / 4;
                }
            }
            Location target;
            double modifier = PI / 8;
            for (int i = 1; i <= Ceiling(5 / degreeModifier); i++)
            {
                double temp = (i / 2) * Pow(-1, i) * modifier;
                target = Cis(radius, baseDegree + temp);
                target.Col += baseLocation.Col;
                target.Row += baseLocation.Row;
                if (game.CanBuildPortalAt(target))
                {
                    //Hide oneself
                    game.Debug("Elf " + builder.Id + " tried to hide. Elf " + builder.AlreadyActed + " acted.");
                    Hide(builder, game);
                    game.Debug("Elf " + builder.Id + " failed? to hide. Elf " + builder.AlreadyActed + " acted.");
                    if (builder.AlreadyActed)
                        return;
                    builder.MoveTo(target);
                    game.Debug("Elf " + builder.Id + " built. Elf " + builder.AlreadyActed + " acted.");
                    return;
                }
            }
            modifier = PI / 40;
            for (int i = 1; i <= Ceiling(25 / degreeModifier); i++)
            {
                double temp = (i / 2) * Pow(-1, i) * modifier;
                target = new Location((int)(radius * Sin(baseDegree + temp)), (int)(radius * Cos(baseDegree + temp)));
                target.Col += baseLocation.Col;
                target.Row += baseLocation.Row;
                if (game.CanBuildPortalAt(target))
                {
                    //Hide oneself
                    game.Debug("Elf " + builder.Id + " tried to hide. Elf " + builder.AlreadyActed + " acted.");
                    Hide(builder, game);
                    game.Debug("Elf " + builder.Id + " failed? to hide. Elf " + builder.AlreadyActed + " acted.");
                    if (builder.AlreadyActed)
                        return;
                    builder.MoveTo(target);
                    game.Debug("Elf " + builder.Id + " built. Elf " + builder.AlreadyActed + " acted.");
                    return;
                }
            }
            BuildInRadius(radius + 150, builder, game);
        }
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
        bool CanBuild(Elf elf, Game game, Building building = Building.Portal)
        {
            switch (building)
            {
                case Building.Portal:
                    if (!elf.CanBuildPortal() || elf.AlreadyActed || elf.Distance(game.GetMyCastle()) < MinPortalBuildRadius)
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
    }
}
