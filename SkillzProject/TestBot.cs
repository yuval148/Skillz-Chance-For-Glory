using ElfKingdom;

/*
 * TO DO:
 * +Run away from snowmans.
 * 
 */

namespace SkillzProject
{
    public class TestBot : ISkillzBot
    {

        /*
         * CONSTS
         */
        const int enemyAggressivePortalRangeFromCastle = 3500;
        const int maxPotentialMana = 600 * 10;
        const int defendRadius = 1300;
        /*
         * END CONSTS
         */

        int totalPortals = 0;
        int buildRange = 1300;
        int manaWasted = 0;
        int turnsWithoutTrolls = 0;
        public void DoTurn(Game game)
        {
            turnsWithoutTrolls++;
            int startingMana = game.GetMyMana();
            Elf[] enemyElves;
            Elf[] myElves = game.GetMyLivingElves();
            Portal[] portals = game.GetMyPortals();
            totalPortals += portals.Length;
            if (game.Turn % 100 == 0)
            {
                buildRange = 2800;
                if (game.Turn < 400)
                {
                    buildRange = 2300;
                }
                if (game.Turn < 200)
                {
                    buildRange = 1800;
                }
            }
            if (game.GetMyCastle().CurrentHealth < 125 && buildRange > 2300)
            {
                buildRange = 2300;
            }
            if (game.GetMyCastle().CurrentHealth < 100 && buildRange > 1800)
            {
                buildRange = 1800;
            }
            if (game.GetMyCastle().CurrentHealth < 50 && buildRange > 1300)
            {
                buildRange = 1300;
            }
            if (portals.Length >= 1)
            {
                if (turnsWithoutTrolls >= 8)
                {
                    bool flag = false;
                    Creature[] enemyGiants = game.GetEnemyLavaGiants();
                    if (enemyGiants != null && !flag)
                    {
                        foreach (Creature giant in enemyGiants)
                        {
                            if (giant.Distance(game.GetMyCastle()) <= 2000)
                            {
                                Portal currentBest = FindNearest(giant, game);
                                if (currentBest.CanSummonIceTroll() && !flag)
                                {
                                    currentBest.SummonIceTroll();
                                    flag = true;
                                }
                                break;
                            }
                        }
                    }
                    enemyElves = game.GetAllEnemyElves();
                    if (enemyElves != null)
                    {
                        foreach (Elf elf in enemyElves)
                        {
                            if (elf.Location == null)
                            {
                                continue;
                            }
                            foreach (Portal current in portals)
                            {
                                if (elf.Distance(current) <= 700)
                                {
                                    if (current.CanSummonIceTroll() && !flag)
                                    {
                                        current.SummonIceTroll();
                                        flag = true;
                                    }
                                }
                            }
                        }
                    }
                    if (enemyElves != null)
                    {
                        foreach (Elf elf in enemyElves)
                        {
                            if (elf.Location == null)
                            {
                                continue;
                            }
                            if (elf.Distance(game.GetMyCastle()) <= 2000)
                            {
                                Portal currentBest = FindNearest(game.GetMyCastle(), game);
                                if (currentBest.CanSummonIceTroll() && !flag)
                                {
                                    currentBest.SummonIceTroll();
                                    flag = true;
                                }
                                break;
                            }
                        }
                    }
                    if (flag)
                    {
                        turnsWithoutTrolls = 0;
                    }
                }
                if (game.Turn % 40 == 1 && game.Turn > 1)
                {
                    Portal currentBest = FindNearest(game.GetEnemyCastle(), game);
                    if (currentBest.CanSummonLavaGiant())
                    {
                        currentBest.SummonLavaGiant();
                    }
                    else if (portals[0].CanSummonLavaGiant())
                    {
                        portals[0].SummonLavaGiant();
                    }
                }
                game.Debug("Average portals: " + (float)totalPortals / game.Turn);
                if ((game.Turn >= 600 && (totalPortals / game.Turn <= portals.Length)) || (game.GetMyCastle().CurrentHealth < 40 && game.GetMyMana() > 50))
                {
                    Portal currentBest = FindNearest(game.GetEnemyCastle(), game);
                    if (currentBest.CanSummonLavaGiant())
                    {
                        currentBest.SummonLavaGiant();
                    }
                    foreach (Portal portal in game.GetMyPortals())
                    {
                        if (portal.CanSummonLavaGiant())
                        {
                            portal.SummonLavaGiant();
                        }
                    }
                }

            }
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
            manaWasted += startingMana - game.GetMyMana();
            game.Debug(manaWasted);
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
        public Portal FindNearest(GameObject gameObject, Game game)
        {
            Portal[] portals = game.GetMyPortals();
            Portal currentBest = portals[0];
            foreach (Portal current in portals)
            {
                if (current.Distance(gameObject) < currentBest.Distance(gameObject))
                {
                    currentBest = current;
                }
            }
            return currentBest;
        }
        Location Cis(double radius, double degree, Location baseLocation = null)
        {
            if (baseLocation == null)
            {
                return new Location((int)(radius * System.Math.Sin(degree)), (int)(radius * System.Math.Cos(degree)));
            }
            else
            {
                return new Location(baseLocation.Row + (int)(radius * System.Math.Sin(degree)), baseLocation.Col + (int)(radius * System.Math.Cos(degree)));
            }
        }
        public double DegreeBetween(Location a, Location b)
        {
            double a1 = System.Math.Sqrt(System.Math.Pow(a.Col - b.Col, 2) + System.Math.Pow(a.Row - b.Row, 2));
            double b1 = System.Math.Abs(a.Col - b.Col);
            return (a.Col > b.Col ? 3 * System.Math.PI / 2 : System.Math.PI / 2) + System.Math.Asin(b1 / a1);
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