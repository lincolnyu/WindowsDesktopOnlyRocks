﻿using System.Linq;
using System.Text;

namespace WindorLib
{
    public class BingMaps
    {
        public static bool IsSortOfAddress(string testee)
        {
            // https://jaspreetchahal.org/australian-street-types-and-abbreviations-database/
            var keywords = new[]
            {
                "ALLEY",
                "AL",
                "AMBLE",
                "AMB",
                "APPROACH",
                "APPR",
                "ARCADE",
                "ARC",
                "ARTERIAL",
                "ART",
                "AVENUE",
                "AV",
                "BAY",
                "BAY",
                "BEND",
                "BEND",
                "BRAE",
                "BRAE",
                "BREAK",
                "BRK",
                "BOULEVARD",
                "BVD",
                "BOARDWALK",
                "BWK",
                "BOWL",
                "BWL",
                "BYPASS",
                "BYP",
                "CIRCLE",
                "CCL",
                "CIRCUS",
                "CCS",
                "CIRCUIT",
                "CCT",
                "CHASE",
                "CHA",
                "CLOSE",
                "CL",
                "CORNER",
                "CNR",
                "COMMON",
                "COM",
                "CONCOURSE",
                "CON",
                "CRESCENT",
                "CR",
                "CROSS",
                "CROS",
                "COURSE",
                "CRSE",
                "CREST",
                "CRST",
                "CRUISEWAY",
                "CRY",
                "COURT/S",
                "CT",
                "COVE",
                "CV",
                "DALE",
                "DALE",
                "DELL",
                "DELL",
                "DENE",
                "DENE",
                "DIVIDE",
                "DIV",
                "DOMAIN",
                "DOM",
                "DRIVE",
                "DR",
                "EAST",
                "EAST",
                "EDGE",
                "EDG",
                "ENTRANCE",
                "ENT",
                "ESPLANADE",
                "ESP",
                "EXTENSION",
                "EXTN",
                "FLATS",
                "FLTS",
                "FORD",
                "FORD",
                "FREEWAY",
                "FWY",
                "GATE",
                "GATE",
                "GARDEN/S",
                "GDN",
                "GLADE/S",
                "GLA",
                "GLEN",
                "GLN",
                "GULLY",
                "GLY",
                "GRANGE",
                "GRA",
                "GREEN",
                "GRN",
                "GROVE",
                "GV",
                "GATEWAY",
                "GWY",
                "HILL",
                "HILL",
                "HOLLOW",
                "HLW",
                "HEATH",
                "HTH",
                "HEIGHTS",
                "HTS",
                "HUB",
                "HUB",
                "HIGHWAY",
                "HWY",
                "ISLAND",
                "ID",
                "JUNCTION",
                "JCT",
                "LANE",
                "LA",
                "LINK",
                "LNK",
                "LOOP",
                "LOOP",
                "LOWER",
                "LWR",
                "LANEWAY",
                "LWY",
                "MALL",
                "MALL",
                "MEW",
                "MEW",
                "MEWS",
                "MWS",
                "NOOK",
                "NOOK",
                "NORTH",
                "NTH",
                "OUTLOOK",
                "OUT",
                "PATH",
                "PATH",
                "PARADE",
                "PD/PDE",
                "POCKET",
                "PKT",
                "PARKWAY",
                "PKW",
                "PLACE",
                "PL",
                "PLAZA",
                "PLZ",
                "PROMENADE",
                "PRM",
                "PASS",
                "PS",
                "PASSAGE",
                "PSG",
                "POINT",
                "PT",
                "PURSUIT",
                "PUR",
                "PATHWAY",
                "PWAY",
                "QUADRANT",
                "QD",
                "QUAY",
                "QU",
                "REACH",
                "RCH",
                "ROAD",
                "RD",
                "RIDGE",
                "RDG",
                "RESERVE",
                "REST",
                "REST",
                "REST",
                "RETREAT",
                "RET",
                "RIDE",
                "RIDE",
                "RISE",
                "RISE",
                "ROUND",
                "RND",
                "ROW",
                "ROW",
                "RISING",
                "RSG",
                "RETURN",
                "RTN",
                "RUN",
                "RUN",
                "SLOPE",
                "SLO",
                "SQUARE",
                "SQ",
                "STREET",
                "ST",
                "SOUTH",
                "STH",
                "STRIP",
                "STP",
                "STEPS",
                "STPS",
                "SUBWAY",
                "SUB",
                "TERRACE",
                "TCE",
                "THROUGHWAY",
                "THRU",
                "TOR",
                "TOR",
                "TRACK",
                "TRK",
                "TRAIL",
                "TRL",
                "TURN",
                "TURN",
                "TOLLWAY",
                "TWY",
                "UPPER",
                "UPR",
                "VALLEY",
                "VLY",
                "VISTA",
                "VST",
                "VIEW/S",
                "VW",
                "WAY",
                "WAY",
                "WOOD",
                "WD",
                "WEST",
                "WEST",
                "WALK",
                "WK",
                "WALKWAY",
                "WKWY",
                "WATERS",
                "WTRS",
                "WATERWAY",
                "WRY",
                "WYND",
                "WY"
            };
            var split = testee.ToUpper().Split(' ', '-', ',', '/', ';', '.');
            return keywords.Any(x => split.Contains(x));
        }

        public static string CreateBingMapsUrl(string addressStr)
        {
            var converted = addressStr.Replace("/", "%2F").Replace(",", "%2C").Replace("-", "%2D");
            var parts = converted.Split(' ').Where(x => x.Length > 0);
            var sb = new StringBuilder();
            foreach (var part in parts)
            {
                sb.Append(part);
                sb.Append('+');
            }
            converted = sb.ToString().TrimEnd('+');
            return $"https://www.bing.com/search?q=%22{converted}%22";
        }
    }
}
