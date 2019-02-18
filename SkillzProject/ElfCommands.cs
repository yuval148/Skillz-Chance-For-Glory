using ElfKingdom;

namespace MyBot
{
    class ElfCommands : StrategicCalculations
    {
        enum Building { Portal, Fountain };
        //To do: after trun 600, elves start attacking
        bool AngryFlag = false;

        public override void DoTurn(Game game)
        {
            Elf[] myElves = game.GetMyLivingElves();
            Portal[] portals = game.GetMyPortals();
            try
            {
                if (game.Turn == 1 && game.GetEnemyLivingElves()[0].Distance(game.GetMyCastle()) < game.GetEnemyLivingElves()[0].Distance(game.GetEnemyCastle()))
                { AngryFlag = true; }
                if (AngryFlag)
                {
                    DefendAgainst(game.GetEnemyPortals(), game, myElves, int.MaxValue, int.MaxValue);
                }
            }
            catch
            {
                game.Debug("We're sad");
            }
            if (myElves.Length < 1)
            {
                return;
            }
            if (game.DefaultManaPerTurn <= 0)
            {
                DefendAgainst(game.GetEnemyManaFountains(), game, myElves, EnemyAggressivePortalRangeFromCastle, EnemyAggressivePortalRangeFromElf);
            }
            if (game.GetMyself().ManaPerTurn <= 8 && !(game.GetMyself().ManaPerTurn == 0 && game.GetMyMana() < game.ManaFountainCost))
            {
                if (myElves[myElves.Length - 1].CanBuildManaFountain())
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
                    DefendAgainst(game.GetAllEnemyElves(), game, myElves, EnemyAggressiveElfRangeFromCastle, EnemyAggressiveElfRangeFromElf);
                    DefendAgainst(game.GetEnemyPortals(), game, myElves, EnemyAggressivePortalRangeFromCastle, EnemyAggressiveElfRangeFromElf);
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
                        else if (game.GetMyself().ManaPerTurn <= 11)
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
            DefendAgainst(game.GetEnemyManaFountains(), game, myElves, 0, game.ElfAttackRange);
            GameObject[] enemyElves = game.GetEnemyLivingElves();
            GameObject[] enemyPortals = game.GetEnemyPortals();
            DefendAgainst(enemyElves, game, myElves, EnemyAggressiveElfRangeFromCastle, EnemyAggressiveElfRangeFromElf);
            DefendAgainst(enemyPortals, game, myElves, EnemyAggressivePortalRangeFromCastle, EnemyAggressivePortalRangeFromElf);
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
                    myElves[i].MoveTo(defult[i]);
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
                    if (builder.AlreadyActed)
                        return;
                    builder.MoveTo(target);
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
                    if (builder.AlreadyActed)
                        return;
                    builder.MoveTo(target);
                    return;
                }
            }
            BuildInRadius(radius + 150, builder, game);
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
