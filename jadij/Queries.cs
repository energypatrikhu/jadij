using MySql.Data.MySqlClient;

namespace jadij
{
    public class Queries
    {
        private readonly string MySqlConnectionString =
            "server=server.lan;database=jadij;user=root;password=;port=13306";
        private readonly MySqlConnection conn;

        public Queries()
        {
            conn = new(MySqlConnectionString);
            conn.Open();

            // 2. feladat
            ExecuteQuery(
                "2016-ban kitüntetettek neve",
                @"SELECT szemely.nev
                FROM szemely
                WHERE szemely.az IN (
                    SELECT kituntetes.szemaz
                    FROM kituntetes
                    WHERE kituntetes.ev = 2016
                )"
            );

            // 3. feladat
            ExecuteQuery(
                "Kritikával foglalkozó díjazottak",
                @"SELECT DISTINCT szemely.nev
                FROM szemely
                WHERE szemely.az IN (
                    SELECT foglalkozas.szemaz
                    FROM foglalkozas
                    WHERE foglalkozas.fognev LIKE '%kritikus%'
                )
                ORDER BY szemely.nev"
            );

            // 4. feladat
            ExecuteQuery(
                "Legalább háromszor kitüntetett személyek",
                @"SELECT szemely.nev, COUNT(*) AS 'Kitüntetések száma'
                FROM szemely
                JOIN kituntetes ON szemely.az = kituntetes.szemaz
                GROUP BY szemely.nev
                HAVING COUNT(*) >= 3"
            );

            // 5. feladat
            ExecuteQuery(
                "A leggyakoribb foglalkozásúak személy neve",
                @"SELECT szemely.nev, foglalkozas.fognev, COUNT(*) AS 'Kitüntetések száma'
                FROM szemely
                JOIN foglalkozas ON szemely.az = foglalkozas.szemaz
                JOIN kituntetes ON szemely.az = kituntetes.szemaz
                GROUP BY foglalkozas.fognev, szemely.nev
                HAVING COUNT(*) = (
                    SELECT MAX(kituntetes_count)
                    FROM (
                        SELECT COUNT(*) AS kituntetes_count
                        FROM szemely
                        JOIN foglalkozas ON szemely.az = foglalkozas.szemaz
                        JOIN kituntetes ON szemely.az = kituntetes.szemaz
                        GROUP BY foglalkozas.fognev, szemely.nev
                    )
                )"
            );

            // 6. feladat
            ExecuteQuery(
                "Bertha Bulcsu-val együtt azok, akik ezekben az években kaptak kitüntetést",
                @"SELECT DISTINCT szemely.nev, kituntetes.ev
                FROM szemely
                JOIN kituntetes ON szemely.az = kituntetes.szemaz
                WHERE kituntetes.ev IN (
                    SELECT kituntetes.ev
                    FROM kituntetes
                    JOIN szemely ON kituntetes.szemaz = szemely.az
                    WHERE szemely.nev = 'Bertha Bulcsu'
                )
                ORDER BY kituntetes.ev, szemely.nev"
            );

            // 7. feladat
            ExecuteQuery(
                "21. évszázad első évtizedében kitüntetettek",
                @"SELECT kituntetes.ev AS 'Évszám', szemely.nev AS 'Név'
                FROM szemely
                JOIN kituntetes ON szemely.az = kituntetes.szemaz
                WHERE kituntetes.ev BETWEEN 2001 AND 2010
                ORDER BY kituntetes.ev, szemely.nev"
            );

            // 8. feladat
            ExecuteQuery(
                "Ritka foglalkozású díjazottak (8ritka)",
                @"SELECT szemely.nev, foglalkozas.fognev
                FROM szemely
                JOIN foglalkozas ON szemely.az = foglalkozas.szemaz
                WHERE foglalkozas.fognev IN (
                    SELECT foglalkozas.fognev
                    FROM foglalkozas
                    GROUP BY foglalkozas.fognev
                    HAVING COUNT(*) < 3
                )"
            );
        }

        private void ExecuteQuery(string comment, string query)
        {
            Console.WriteLine(
                $"\n---------------------------------------------------------------------------------------------------\n\t{comment}"
            );

            MySqlCommand cmd = new(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            // Write column names
            for (int i = 0; i < reader.FieldCount; i++)
            {
                Console.Write(reader.GetName(i) + " ");
            }

            // divider
            Console.WriteLine();

            // Write rows
            while (reader.Read())
            {
                Console.WriteLine();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.Write(reader.GetValue(i) + " ");
                }
            }

            reader.Close();
        }
    }
}
