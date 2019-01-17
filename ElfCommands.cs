using ElfKingdom;

namespace SkillzProject
{
    class ElfCommands : StrategicCalculations
    {
        public override void DoTurn(Game game)
        {
            Elf[] myElves = game.GetMyLivingElves();
            Portal[] portals = game.GetMyPortals();
            if (myElves.Length < 1)
            {
                return;
            }
            if (PortalsInRadius(1300, game) >= 4)
            {
                DefendAgainst(game.GetAllEnemyElves(), game, myElves, 0, 300);
                DefendAgainst(game.GetEnemyPortals(), game, myElves, enemyAggressivePortalRangeFromCastle, 700);
            }
            else
            {
                if (game.GetMyMana() > 100)
                {
                    if (myElves[0].CanBuildPortal() && !myElves[0].AlreadyActed)
                    {
                        myElves[0].BuildPortal();
                    }
                    else
                    {
                        BuildInRadius(1300, myElves[0], game);
                    }
                }
            }
            if (myElves.Length >= 1 && !myElves[0].AlreadyActed)
            {
                if (game.GetMyMana() > 100 && PortalsInRadius(buildRange, game) < 5)
                {
                    if (myElves[0].CanBuildPortal() && !myElves[0].AlreadyActed)
                    {
                        myElves[0].BuildPortal();
                    }
                    else
                    {
                        BuildInRadius(buildRange, myElves[0], game);
                    }
                }
            }
            DefendAgainst(game.GetAllEnemyElves(), game, myElves, 0, 300);
            DefendAgainst(game.GetEnemyPortals(), game, myElves, enemyAggressivePortalRangeFromCastle, 700);
            DefendAgainst(game.GetAllEnemyElves(), game, myElves, 1500, 500);
            DefendAgainst(game.GetEnemyLavaGiants(), game, myElves);
            //Defult 1 - defend portals
            DefendOn(game.GetMyPortals(), game.GetEnemyLivingElves(), myElves, 750);
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
                    myElves[i].MoveTo(Cis(defendRadius, degree, game.GetMyCastle().Location));
                }
            }
            //Defult 3 - pretend you're working
            foreach (var elf in myElves)
            {
                if (elf.AlreadyActed)
                {
                    continue;
                }
                if (game.GetMyMana() > 100 && PortalsInRadius(1300, game) < 5 && elf.CanBuildPortal())
                {
                    elf.BuildPortal();
                    continue;
                }
                BuildInRadius(buildRange, elf, game);
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
        public void BuildInRadius(double radius, Elf builder, Game game, double degree = System.Math.PI / 3)
        {
            if (builder.AlreadyActed)
                return;
            if (manaWasted >= 3000)
            {
                if (radius > maxPotentialMana - manaWasted && radius > 1700)
                {
                    return;
                }
            }
            if (radius > 5000) return;
            Location baseLocation = game.GetMyCastle().Location;
            double baseDegree;
            double a = System.Math.Sqrt(System.Math.Pow(baseLocation.Col - game.GetEnemyCastle().Location.Col, 2) + System.Math.Pow(baseLocation.Row - game.GetEnemyCastle().Location.Row, 2));
            double b = System.Math.Abs(baseLocation.Col - game.GetEnemyCastle().Location.Col);
            baseDegree = System.Math.Asin(b / a);
            baseDegree = System.Math.PI / 4;
            int min = int.MaxValue;
            for (int i = 0; i < 4; i++)
            {
                if (game.GetEnemyCastle().Distance(Cis(100, System.Math.PI / 4 + System.Math.PI * i / 2, game.GetMyCastle().Location)) < min)
                {
                    min = game.GetEnemyCastle().Distance(Cis(100, System.Math.PI / 4 + System.Math.PI * i / 2, game.GetMyCastle().Location));
                    baseDegree = System.Math.PI / 4 + System.Math.PI * i / 2;
                }
            }
            Location target;
            double modifier = System.Math.PI / 8;
            for (int i = 1; i <= 5; i++)
            {
                double temp = (i / 2) * System.Math.Pow(-1, i) * modifier;
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
            modifier = System.Math.PI / 40;
            for (int i = 1; i <= 25; i++)
            {
                double temp = (i / 2) * System.Math.Pow(-1, i) * modifier;
                target = new Location((int)(radius * System.Math.Sin(baseDegree + temp)), (int)(radius * System.Math.Cos(baseDegree + temp)));
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
    }
}
