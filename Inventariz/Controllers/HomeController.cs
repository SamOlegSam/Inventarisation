using Inventariz.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ClosedXML.Excel;
using ClosedXML.Report;
using Newtonsoft.Json;
using System.Web.UI.DataVisualization.Charting;
using System.Data.OleDb;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Data;
using System.Data.Entity;
using System.Net.Http;
//using System.Text.Json;
//using Newtonsoft.Json;


namespace Inventariz.Controllers
{

    //--------------------------------------------------------------------------------------------------------------------------------
    public class TankInventt
    {
        public DateTime Data { get; set; }
        public int Filial { get; set; }
        public int Rezer { get; set; }
        public int? Urov { get; set; }
        public int? UrovH2O { get; set; }
        public int? UrovNeft { get; set; }
        public double V { get; set; }
        public double VH2O { get; set; }
        public double VNeft { get; set; }
        public double Temp { get; set; }
        public double P { get; set; }
        public double MassaBrutto { get; set; }
        public double H2O { get; set; }
        public double Salt { get; set; }
        public double Meh { get; set; }
        public double BalProc { get; set; }
        public double BalTonn { get; set; }
        public double MassaNetto { get; set; }
        public double Hmin;
        public double Vmin;
        public double dMBalmin;
        public double dMNettomin;
        public double MBalTeh { get; set; }
        public double MNettoTeh { get; set; }
        public int type { get; set; }

    }

    public class CalculatorData
    {
        /// <summary>
        /// Oil density at a temperature of 15 °C.
        /// </summary>
        public double Dens15 { get; set; }

        /// <summary>
        /// Oil density at a temperature of 20 °C.
        /// </summary>
        public double Dens20 { get; set; }

        /// <summary>
        /// Values of the coefficient K20 / K15.
        /// </summary>
        public double K20_K15 { get; set; }

        /// <summary>
        /// Values of oil expansion coefficients.
        /// </summary>
        public double Betta { get; set; }

        /// <summary>
        /// Value of oil compressibility ratios.
        /// </summary>
        public double Gamma { get; set; }

        /// <summary>
        /// Adduction density.
        /// </summary>
        public double DensTP { get; set; }
    }

    public class Density636 : ICloneable
    {
        public double DensAreom { get; set; }
        public double TempAreom { get; set; }
        public double GradAreom { get; set; }
        public double TempReal { get; set; }
        public double Pressure { get; set; }

        public Density636()
        {
            DensAreom = 0;
            TempAreom = 0;
            GradAreom = 20;
            TempReal = 0;
            Pressure = 0;
        }

        public object Clone()
        {
            return new Density636
            {
                DensAreom = this.DensAreom,
                TempAreom = this.TempAreom,
                GradAreom = this.GradAreom,
                TempReal = this.TempReal,
                Pressure = this.Pressure
            };
        }

    }

    //--------------------------------------------------------------------------------------------------------------------------------

    public class Asopn
    {
        public static asopnEntities db1 = new asopnEntities();
        public static ChemLabEntities db2 = new ChemLabEntities();

        public static List<LastTanksResultMoz> LM = new List<LastTanksResultMoz>();  //список записей из представления химанализа по Мозырю
        public static List<LastTanksResultPol> LP = new List<LastTanksResultPol>();  //список записей из представления по Полоцку
        public static List<LastMechanResultMoz> LMM = new List<LastMechanResultMoz>(); //список записей механических примесей в резервуарах ЛПДС Мозырь

        public static LastTanksResultMoz LastM = new LastTanksResultMoz();  //экземпляр класса химанализ Мозыря
        public static LastTanksResultPol LastP = new LastTanksResultPol();  //экземпляр класса химанализ Полоцка
        public static LastMechanResultMoz LastMM = new LastMechanResultMoz(); //экземпляр класса механических примесей Мозыря


        public static List<TankInventt> TankI = new List<TankInventt>();

        public static List<tankinfo> tankinfos = new List<tankinfo>();
        public static taginfo TI = new taginfo();

        public static trl_tank trl = new trl_tank();

        static List<calibration> calibrationList = new List<calibration>();

        static List<TankInv> ListInv = new List<TankInv>();
        static List<TankInv> ListLastInv = new List<TankInv>();


        public async static Task<List<TankInventt>> Spis(int filial2, DateTime dataHim, DateTime dataInvent, string timeInvent)
        {
            TI = db1.taginfo.OrderByDescending(h => h.dt).FirstOrDefault();
            // tankinfos = db1.tankinfo.Where(j => j.dt == TI.dt).OrderBy(j => j.filialid).ThenBy(j => j.tankid).ToList();
            //DateTime DateTimeInv = Convert.ToDateTime((Convert.ToString(dataInvent)).Replace("00:00:00", timeInvent));
            DateTime DateTimeInv = Convert.ToDateTime(dataInvent.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss").Replace("00:00:00", timeInvent));
            DateTime DateTimeHim = Convert.ToDateTime(dataHim.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss").Replace("00:00:00", "23:59:00"));

            tankinfos = db1.tankinfo.Where(j => j.dt == DateTimeInv && j.filialid == filial2).OrderBy(j => j.filialid).ThenBy(j => j.tankid).ToList();

            //var LP = db2.LastTanksResultPol;
            LP = db2.LastTanksResultPol.ToList();
            LM = db2.LastTanksResultMoz.ToList();
            LMM = db2.LastMechanResultMoz.ToList();


            List<DayParkResultMoz_Result> DPRM = new List<DayParkResultMoz_Result>();
            List<DayParkResultPol_Result> DPRP = new List<DayParkResultPol_Result>();
            List<DayParkMechanMoz_Result> MehMoz = new List<DayParkMechanMoz_Result>();
            List<DayParkMechanPol_Result> MehPol = new List<DayParkMechanPol_Result>();

            DPRM = db2.DayParkResultMoz(DateTimeHim).ToList();
            DPRP = db2.DayParkResultPol(DateTimeHim).ToList();
            MehMoz = db2.DayParkMechanMoz(DateTimeHim).ToList();
            MehPol = db2.DayParkMechanPol(DateTimeHim).ToList();

            ListInv = db1.TankInv.OrderByDescending(g => g.Data).ToList();
            DateTime LastDataInv = ListInv.FirstOrDefault().Data; //Получаем дату последней инвентаризации
            ListLastInv = ListInv.Where(f => f.Data == LastDataInv).ToList(); //Получаем список инвентаризации за последнюю дату;

            //Создадим экземпляр класса и список параметров для корректировки автоматически создаваемой инвентаризации
            Correct corr = new Correct();
            List<Correct> corrList = new List<Correct>();
            corrList = db1.Correct.ToList();

            calibrationList = db1.calibration.OrderBy(f => f.oillevel).ToList();
            decimal? UrovH2O;
            double UrovNeft;
            double V;
            double VH2O;
            double VNeft;
            double Umin;
            double Umax;
            double Vmin;
            double Vmax;
            double Upercent;
            double UminH2O;
            double UmaxH2O;
            double VminH2O;
            double VmaxH2O;
            double UpercentH2O;
            double Temp1;
            double P;
            double Psalt;
            double MassaBrutto;
            double H2O;
            double Salt;
            double Meh;
            double BalProc;
            double BalTonn;
            double MassaNetto;
            double Hmin;
            double MVmin;
            double MMin;
            double MNmin;
            int type;
            double VRasch;
            double VH2ORasch;
            double Kst;
            decimal? lev = 0;

            TankI.Clear(); //Очищаем TankI, а то каждый раз добаляется список резервуаров к старому списку!!!!!!!

            foreach (var item in tankinfos)
            {
                //Проверяем тип резервуара (ЖБР или РВС)
                if (item.tankid == 1 | item.tankid == 2 | item.tankid == 3 | item.tankid == 4 | item.tankid == 5 | item.tankid == 6 | item.tankid == 7 | item.tankid == 8 | item.tankid == 9 | item.tankid == 10)
                {
                    Kst = 0.00001;
                }
                else
                {
                    Kst = 0.0000125;
                }
                decimal? temper;
                if (ListLastInv.FirstOrDefault(d => d.Filial == item.filialid & d.Rezer == item.tankid) == null)
                {
                    UrovH2O = 0;
                }
                else
                {
                    UrovH2O = ListLastInv.FirstOrDefault(d => d.Filial == item.filialid & d.Rezer == item.tankid).UrovH2O;
                }

                //Вывод общего объема нефти и подтоварной воды согласно общего уровня-------------------------------------------------------------
                if (db1.trl_tank.FirstOrDefault(g => g.TankID == item.tankid & g.FilialID == item.filialid) == null)
                {
                    type = 77;
                }
                else
                {
                    type = db1.trl_tank.FirstOrDefault(g => g.TankID == item.tankid & g.FilialID == item.filialid).TypeID;
                }

                if (calibrationList.LastOrDefault(g => g.tankid == item.tankid & g.filialid == item.filialid) == null || calibrationList.FirstOrDefault(j => j.tankid == item.tankid & j.filialid == item.filialid) == null)
                {
                    V = 0;
                    VRasch = 0;
                    Temp1 = 0;
                }
                else
                {

                    //if (calibrationList.LastOrDefault(g => g.tankid == item.tankid & g.oillevel <= item.level & g.filialid == item.filialid) == null)
                    //{
                    //    Umin = 0;
                    //}
                    //else
                    //{
                    //    Umin = calibrationList.LastOrDefault(g => g.tankid == item.tankid & g.oillevel <= item.level & g.filialid == item.filialid).oillevel;
                    //}
                    //Umax = calibrationList.FirstOrDefault(j => j.tankid == item.tankid & j.oillevel > item.level & j.filialid == item.filialid).oillevel;
                    //if (calibrationList.LastOrDefault(g => g.tankid == item.tankid & g.oillevel <= item.level & g.filialid == item.filialid) == null)
                    //{
                    //    Vmin = 0;
                    //}
                    //else
                    //{
                    //    Vmin = calibrationList.LastOrDefault(g => g.tankid == item.tankid & g.oillevel <= item.level & g.filialid == item.filialid).oilvolume;
                    //}

                    //Vmax = calibrationList.FirstOrDefault(j => j.tankid == item.tankid & j.oillevel > item.level & j.filialid == item.filialid).oilvolume;
                    //Upercent = (Convert.ToDouble(item.level) - Umin) / (Umax - Umin);
                    //V = Vmin + (Vmax - Vmin) * Upercent;
                    //Temp1 = Convert.ToDouble(item.t);

                    ////Рассчитываем общий объем (нефть и подтоварная вода) по формуле
                    //VRasch = V * (1 + 2 * Kst * (Temp1 - 20));

                    {
                        if (corrList.FirstOrDefault(g => g.filial == item.filialid & g.tankid == item.tankid) == null)
                        {
                            lev = item.level;
                            temper = Convert.ToDecimal(item.t);
                            //------если корректирующая таблица пустая, берем данные из танкрадара---------------------------------------------------------------
                            if (calibrationList.LastOrDefault(g => g.tankid == item.tankid & g.oillevel <= lev & g.filialid == item.filialid) == null)
                            {
                                Umin = 0;
                            }
                            else
                            {
                                Umin = calibrationList.LastOrDefault(g => g.tankid == item.tankid & g.oillevel <= lev & g.filialid == item.filialid).oillevel;
                            }
                            Umax = calibrationList.FirstOrDefault(j => j.tankid == item.tankid & j.oillevel > lev & j.filialid == item.filialid).oillevel;
                            if (calibrationList.LastOrDefault(g => g.tankid == item.tankid & g.oillevel <= lev & g.filialid == item.filialid) == null)
                            {
                                Vmin = 0;
                            }
                            else
                            {
                                Vmin = calibrationList.LastOrDefault(g => g.tankid == item.tankid & g.oillevel <= lev & g.filialid == item.filialid).oilvolume;
                            }

                            Vmax = calibrationList.FirstOrDefault(j => j.tankid == item.tankid & j.oillevel > lev & j.filialid == item.filialid).oilvolume;
                            Upercent = (Convert.ToDouble(item.level) - Umin) / (Umax - Umin);

                            V = Vmin + (Vmax - Vmin) * Upercent;
                            Temp1 = Convert.ToDouble(temper);

                            //Рассчитываем общий объем (нефть и подтоварная вода) по формуле
                            VRasch = V * (1 + 2 * Kst * (Temp1 - 20));

                        }
                        //---если запись в корректирующей таблице есть-----------------------------------------------------------------------------------------
                        else
                        {
                            if (corrList.FirstOrDefault(g => g.filial == item.filialid & g.tankid == item.tankid).Uroven == null)
                            {
                                lev = Convert.ToDecimal(item.level);
                            }
                            else
                            {
                                lev = corrList.FirstOrDefault(g => g.filial == item.filialid & g.tankid == item.tankid).Uroven;
                            }
                            if (corrList.FirstOrDefault(g => g.filial == item.filialid & g.tankid == item.tankid).Temp == null)
                            {
                                temper = Convert.ToDecimal(item.t);
                            }
                            else
                            {
                                temper = corrList.FirstOrDefault(g => g.filial == item.filialid & g.tankid == item.tankid).Temp;
                            }

                            //---------------------------------------------------------------
                            if (calibrationList.LastOrDefault(g => g.tankid == item.tankid & g.oillevel <= lev & g.filialid == item.filialid) == null)
                            {
                                Umin = 0;
                            }
                            else
                            {
                                Umin = calibrationList.LastOrDefault(g => g.tankid == item.tankid & g.oillevel <= lev & g.filialid == item.filialid).oillevel;
                            }
                            Umax = calibrationList.FirstOrDefault(j => j.tankid == item.tankid & j.oillevel > lev & j.filialid == item.filialid).oillevel;
                            if (calibrationList.LastOrDefault(g => g.tankid == item.tankid & g.oillevel <= lev & g.filialid == item.filialid) == null)
                            {
                                Vmin = 0;
                            }
                            else
                            {
                                Vmin = calibrationList.LastOrDefault(g => g.tankid == item.tankid & g.oillevel <= lev & g.filialid == item.filialid).oilvolume;
                            }

                            Vmax = calibrationList.FirstOrDefault(j => j.tankid == item.tankid & j.oillevel > lev & j.filialid == item.filialid).oilvolume;
                            Upercent = (Convert.ToDouble(lev) - Umin) / (Umax - Umin);

                            V = Vmin + (Vmax - Vmin) * Upercent;
                            Temp1 = Convert.ToDouble(temper);

                            //Рассчитываем общий объем (нефть и подтоварная вода) по формуле
                            VRasch = V * (1 + 2 * Kst * (Temp1 - 20));
                            //----------------------------------------------------------------

                        }
                    }


                }

                //-------Вывод объема подтоварной воды--------------------------------------------------------------------------------------------------------
                //
                if (calibrationList.LastOrDefault(g => g.tankid == item.tankid & g.filialid == item.filialid) == null || calibrationList.FirstOrDefault(j => j.tankid == item.tankid & j.filialid == item.filialid) == null || ListLastInv.FirstOrDefault(d => d.Filial == item.filialid & d.Rezer == item.tankid) == null || ListLastInv.FirstOrDefault(d => d.Filial == item.filialid & d.Rezer == item.tankid).UrovH2O == 0)
                {
                    VH2O = 0;
                    VH2ORasch = 0;
                }
                else
                {
                    if (calibrationList.LastOrDefault(g => g.tankid == item.tankid & g.oillevel <= ListLastInv.FirstOrDefault(d => d.Filial == item.filialid & d.Rezer == item.tankid).UrovH2O & g.filialid == item.filialid) == null)
                    {
                        UminH2O = 0;
                    }
                    else
                    {
                        UminH2O = calibrationList.LastOrDefault(g => g.tankid == item.tankid & g.oillevel <= ListLastInv.FirstOrDefault(d => d.Filial == item.filialid & d.Rezer == item.tankid).UrovH2O & g.filialid == item.filialid).oillevel;
                    }
                    UmaxH2O = calibrationList.FirstOrDefault(j => j.tankid == item.tankid & j.oillevel > ListLastInv.FirstOrDefault(d => d.Filial == item.filialid & d.Rezer == item.tankid).UrovH2O & j.filialid == item.filialid).oillevel;
                    if (calibrationList.LastOrDefault(g => g.tankid == item.tankid & g.oillevel <= ListLastInv.FirstOrDefault(d => d.Filial == item.filialid & d.Rezer == item.tankid).UrovH2O & g.filialid == item.filialid) == null)
                    {
                        VminH2O = 0;
                    }
                    else
                    {
                        VminH2O = calibrationList.LastOrDefault(g => g.tankid == item.tankid & g.oillevel <= ListLastInv.FirstOrDefault(d => d.Filial == item.filialid & d.Rezer == item.tankid).UrovH2O & g.filialid == item.filialid).oilvolume;
                    }

                    VmaxH2O = calibrationList.FirstOrDefault(j => j.tankid == item.tankid & j.oillevel > ListLastInv.FirstOrDefault(d => d.Filial == item.filialid & d.Rezer == item.tankid).UrovH2O & j.filialid == item.filialid).oilvolume;
                    UpercentH2O = (Convert.ToDouble(ListLastInv.FirstOrDefault(d => d.Filial == item.filialid & d.Rezer == item.tankid).UrovH2O) - UminH2O) / (UmaxH2O - UminH2O);
                    VH2O = VminH2O + (VmaxH2O - VminH2O) * UpercentH2O;

                    //Рассчитываем объем подтоварной воды по формуле
                    VH2ORasch = VH2O * (1 + 2 * Kst * (Temp1 - 20));
                }
                // Уровень нефти
                //VNeft = V - VH2O; Закоментировал объем нефти по калибровочной таблице
                //Рассчитываем объем нефти по формуле.
                VNeft = VRasch - VH2ORasch;
                //---------------------------------------------------------------------------------------------------------------
                //Вывод плотности нефти при температуре 20----------------------------------------------------------------------------
                //Temp = 0;
                P = 0;
                Psalt = 0;
                H2O = 0;
                Salt = 0;
                Meh = 0;

                if (filial2 == 1)
                {
                    foreach (var i in DPRM.Where(f => f.tankid == item.tankid))
                    {

                        P = i.densreal.Value;
                        Psalt = i.densreal.Value;
                        if (i.water == null)
                        {
                            H2O = 0;
                        }
                        else
                        {
                            H2O = i.water.Value;
                        }
                        if (i.saltmg == null)
                        {
                            Salt = 0;
                        }
                        else
                        {
                            Salt = i.saltmg.Value;
                        }
                        // записываем значение механических примесей резервуара ЛПДС Мозырь
                        if (MehMoz.FirstOrDefault(g => g.tankid == item.tankid) == null)
                        {
                            Meh = 0;
                        }
                        else
                        {
                            Meh = MehMoz.FirstOrDefault(g => g.tankid == item.tankid).mechan.Value;
                        }

                    }
                }
                else if (filial2 == 2)
                {
                    foreach (var i in DPRP.Where(f => f.tankid == item.tankid))
                    {

                        P = i.densreal.Value;
                        Psalt = i.densreal.Value;
                        if (i.water == null)
                        {
                            H2O = 0;
                        }
                        else
                        {
                            H2O = i.water.Value;
                        }
                        if (i.saltmg == null)
                        {
                            Salt = 0;
                        }
                        else
                        {
                            Salt = i.saltmg.Value;
                        }
                        // записываем значение механических примесей резервуара ЛПДС Мозырь
                        if (MehPol.FirstOrDefault(g => g.tankid == item.tankid) == null)
                        {
                            Meh = 0;
                        }
                        else
                        {
                            Meh = MehPol.FirstOrDefault(g => g.tankid == item.tankid).mechan.Value;
                        }
                    }
                }

                //-----------------------------------------------------------
                var dens636 = new Density636()
                {
                    DensAreom = P,
                    TempAreom = 20,
                    GradAreom = 20,
                    TempReal = Temp1,
                    Pressure = 0
                };
                var json = JsonConvert.SerializeObject(dens636);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                string urlDensCalc = "http://192.168.150.144:88/denscalc";
                var clientDensCalc = new HttpClient();


                var response = await clientDensCalc.PostAsync(urlDensCalc, data);

                string result = response.Content.ReadAsStringAsync().Result;
                var calculatorData = JsonConvert.DeserializeObject<CalculatorData>(result);
                double PCalc;
                if (calculatorData == null)
                {
                    PCalc = 0;
                }
                else
                {
                    PCalc = calculatorData.DensTP;
                }
                double SaltProc;
                if (PCalc == 0)
                {
                    SaltProc = 0;
                }
                else
                {
                    SaltProc = Salt / Psalt / 10;
                }
                //-----------------------------------------------------------

                MassaBrutto = PCalc * VNeft / 1000;
                BalProc = H2O + SaltProc + +Meh;
                BalTonn = MassaBrutto * BalProc / 100;
                MassaNetto = MassaBrutto - BalTonn;
                if (db1.trl_tank.FirstOrDefault(f => f.FilialID == item.filialid & f.TankID == item.tankid) == null)
                {
                    Hmin = 0;
                }
                else
                {
                    Hmin = Convert.ToDouble(db1.trl_tank.FirstOrDefault(f => f.FilialID == item.filialid & f.TankID == item.tankid).MinDopLevel);
                }
                if (db1.trl_tank.FirstOrDefault(f => f.FilialID == item.filialid & f.TankID == item.tankid) == null)
                {
                    MVmin = 0;
                }
                else
                {
                    MVmin = Convert.ToDouble(db1.trl_tank.FirstOrDefault(f => f.FilialID == item.filialid & f.TankID == item.tankid).MinDopVol);
                }

                MMin = PCalc * MVmin / 1000;
                MNmin = MMin - (MMin * BalProc / 100);
                //MNmin = MMin * (100 - BalProc) / 100;
                //VH2O = 0; //Задаем значение объема подтоварной воды = 0, т.к. уровень подтоварной воды нигде не указывается
                UrovNeft = Convert.ToDouble(item.level) - Convert.ToDouble(UrovH2O);
                //VNeft = V - VH2O;
                //---------------------------------------------------------------------------------------------------------------
                TankI.Add(new TankInventt() { Data = item.dt, Filial = item.filialid, Rezer = item.tankid, Urov = item.level, UrovH2O = Convert.ToInt32(UrovH2O), UrovNeft = Convert.ToInt32(UrovNeft), V = V, P = PCalc, Temp = Temp1, MassaBrutto = MassaBrutto, H2O = H2O, Salt = SaltProc, Meh = Meh, BalProc = BalProc, BalTonn = BalTonn, MassaNetto = MassaNetto, Hmin = Hmin, Vmin = MVmin, dMBalmin = MMin, dMNettomin = MNmin, type = type, VH2O = VH2O, VNeft = VNeft }); ;
            }
            return TankI;
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------------

    public class HIM
    {
        public DateTime? dat;
        public int? rezer;
        public double? densreal;
        public double? dens20;
        public double? tempreal;
        public double? water;
        public double? saltmg;
        public double? mechan;
    }
    public class TankInfoType
    {
        public int tankid;
        public DateTime date;
        public int filial;
        public double level;
        public double temp;
        public int type;
    }


    public class HomeController : Controller
    {


        public asopnEntities db = new asopnEntities();
        public ChemLabEntities db2 = new ChemLabEntities();
        trl_tankview vi = new trl_tankview();

        public ActionResult Index()
        {
            string Hour = DateTime.Now.Hour.ToString();
            string Dat = DateTime.Now.ToShortDateString().ToString();
            string DatHour = Hour + ":00" + "; " + Dat;
            ViewBag.DatHour = DatHour;

            //----Получаем суммарный объем нефти в РП Мозырь-------------------

            List<TankInv> TableInv = new List<TankInv>();
            List<TankInv> TableInvNov = new List<TankInv>();
            DateTime las = db.TankInv.Max(d => d.Data);
            TableInv = db.TankInv.Where(j => j.Filial == 1 & j.Data == las).ToList();
            TableInvNov = db.TankInv.Where(j => j.Filial == 2 & j.Data == las).ToList(); ;


            double VNeftItog1 = 0;
            double VNeftItog2 = 0;

            foreach (var t in TableInv)
            {
                VNeftItog1 = VNeftItog1 + Convert.ToDouble(t.VNeft);
            }
            //---------------------------------------------
            ViewBag.VNeftItog1 = VNeftItog1;

            foreach (var t in TableInvNov)
            {
                VNeftItog2 = VNeftItog2 + Convert.ToDouble(t.VNeft);
            }
            //---------------------------------------------
            ViewBag.VNeftItog2 = VNeftItog2;
            ViewBag.SumNeft = VNeftItog1 + VNeftItog2;

            //-----------------------------------------------------------------

            return View();
        }
        //--------------Добавлен расчет балласта----------------------------------------
        public ActionResult Calc()
        {

            SelectList podrazdD = new SelectList(db.filials, "id", "name");
            ViewBag.podr = podrazdD;

            SelectList RezMoz = new SelectList(db.trl_tankview, "TankID", "TankName");
            ViewBag.rezer = RezMoz;


            return View();
        }

        public ActionResult GradTables()
        {
            SelectList podrazdD = new SelectList(db.filials, "id", "name");
            ViewBag.podr = podrazdD;

            SelectList RezMoz = new SelectList(db.trl_tankview, "TankID", "TankName");
            ViewBag.rezer = RezMoz;

            return View();
        }

        public ActionResult AktInvent()
        {
            SelectList podrazdD1 = new SelectList(db.filials, "id", "name");
            ViewBag.podr1 = podrazdD1;

            //List<TankInv> DatINV = new List<TankInv>();
            //DatINV = db.TankInv.Where(j => j.Filial == 1).OrderByDescending(h => h.Data).ToList();
            //List<TankInv> DatInv = new List<TankInv>();

            //var maxDatG = db.TankInv.Where(k => k.Filial == 1).Max(y => y.Data);
            //DatInv = db.TankInv.Where(j => j.Filial == 1 & j.Data == maxDatG).OrderBy(h => h.Rezer).ToList();

            //SelectList inventGomel = new SelectList(db.filials, "id", "name");
            //ViewBag.datinv = DatInv;

            //List<DateTime?> datSelect = new List<DateTime?>();

            //foreach (var item in DatINV)
            //{
            //    datSelect.Add(item.Data);
            //}
            //List<DateTime?> datSelectDiST = new List<DateTime?>();
            //datSelectDiST = datSelect.Distinct().ToList();
            //ViewBag.SelectDist = datSelectDiST;

            return View();
        }

        public ActionResult NormInformation()
        {

            return View();
        }


        //---------Резервуары------------------------------------
        public ActionResult RezerMoz()
        {
            List<trl_tank> RezerMoz = new List<trl_tank>();
            RezerMoz = db.trl_tank.Where(g => g.FilialID == 1).OrderBy(d => d.TankID).ToList();

            ViewBag.RezerMoz = RezerMoz;

            return View(RezerMoz);
        }

        public ActionResult RezerNovop()
        {
            List<trl_tank> RezerNovop = new List<trl_tank>();
            RezerNovop = db.trl_tank.Where(s => s.FilialID == 2).OrderBy(a => a.TankID).ToList();

            ViewBag.RezerNovop = RezerNovop;

            return View(RezerNovop);
        }

        //----------------------------------------------------

        //----------------Подписанты--------------------------//
        public ActionResult Kom()
        {
            List<Podpisanty> podpisanty = new List<Podpisanty>();
            podpisanty = db.Podpisanty.OrderBy(h => h.IDFilial).ToList();

            SelectList komis = new SelectList(db.filials, "id", "name");
            ViewBag.komis = komis;

            return View(podpisanty);
        }

        public ActionResult KomMoz()
        {
            List<Podpisanty> podpisantyMoz = new List<Podpisanty>();
            podpisantyMoz = db.Podpisanty.Where(f => f.IDFilial == 1).ToList();

            SelectList komisMoz = new SelectList(db.filials, "id", "name");
            ViewBag.komisMoz = komisMoz;

            return View(podpisantyMoz);
        }

        public ActionResult KomNovop()
        {
            List<Podpisanty> podpisantyNovop = new List<Podpisanty>();
            podpisantyNovop = db.Podpisanty.Where(f => f.IDFilial == 2).ToList();

            SelectList komisNovop = new SelectList(db.filials, "id", "name");
            ViewBag.komisNovop = komisNovop;

            return View(podpisantyNovop);
        }

        //----------Добавление члена комиссии-----------------------//

        public ActionResult AddKomissiya()
        {
            SelectList komis = new SelectList(db.filials, "id", "name");
            ViewBag.komis = komis;
            return PartialView();
        }
        //--------------------------//
        //-----Сохранение добавления члена комиссии-----------//
        [HttpPost]
        public ActionResult KomissiyaSave(string location, string Nazn, string Doljnost, string FIO)
        {
            try
            {
                Podpisanty kom = new Podpisanty();
                kom.IDFilial = Convert.ToInt32(location);
                kom.Nazn = Nazn.Trim();
                kom.Doljnost = Doljnost.Trim();
                kom.FIO = FIO.Trim();
                db.Podpisanty.Add(kom);

                db.SaveChanges();

                ViewBag.Message = "Член комиссии успешно добавлендобавлен!";
            }
            catch (Exception ex)
            {


                ViewBag.Message = ex.ToString();
            }

            return PartialView();
        }

        //-Добавление члена комиссии для Мозыря-------//

        public ActionResult AddKomissiyaMoz()
        {
            return PartialView();
        }
        //--------------------------//
        //-----Сохранение добавления члена комиссии-----------//
        [HttpPost]
        public ActionResult KomissiyaSaveMoz(string NaznM, string DoljnostM, string FIOM)
        {
            try
            {
                Podpisanty komM = new Podpisanty();
                komM.IDFilial = 1;
                komM.Nazn = NaznM.Trim();
                komM.Doljnost = DoljnostM.Trim();
                komM.FIO = FIOM.Trim();
                db.Podpisanty.Add(komM);

                db.SaveChanges();

                ViewBag.Message = "Член комиссии успешно добавлендобавлен!";
            }
            catch (Exception ex)
            {


                ViewBag.Message = ex.ToString();
            }

            return PartialView();
        }


        //-----Добавление члена комиссии Полоцка---------//

        public ActionResult AddKomissiyaNovop()
        {
            return PartialView();
        }
        //--------------------------//
        //-----Сохранение добавления члена комиссии-----------//
        [HttpPost]
        public ActionResult KomissiyaSaveNovop(string NaznN, string DoljnostN, string FION)
        {
            try
            {
                Podpisanty komN = new Podpisanty();
                komN.IDFilial = 2;
                komN.Nazn = NaznN.Trim();
                komN.Doljnost = DoljnostN.Trim();
                komN.FIO = FION.Trim();
                db.Podpisanty.Add(komN);

                db.SaveChanges();

                ViewBag.Message = "Член комиссии успешно добавлендобавлен!";
            }
            catch (Exception ex)
            {


                ViewBag.Message = ex.ToString();
            }

            return PartialView();
        }


        //-----------------------------------

        // удаление подписанта//

        public ActionResult DeletePodpis(int ID)
        {
            Podpisanty podp = new Podpisanty();
            podp = db.Podpisanty.FirstOrDefault(a => a.ID == ID);
            return PartialView(podp);
        }
        //-----------------------------//

        // Подтверждение удаления подписанта//
        public ActionResult DeletePodpisOK(int ID)
        {
            try
            {

                Podpisanty podpis = new Podpisanty();
                podpis = db.Podpisanty.FirstOrDefault(a => a.ID == ID);
                db.Podpisanty.Remove(podpis);
                db.SaveChanges();

                ViewBag.Message = "Подписант успешно удален!";
            }
            catch
            {
                ViewBag.Message = "Ошибка удаления";
            }

            return PartialView();
        }
        // Редактирование члена комиссии//

        public ActionResult KomissiyaEdit(int ID)
        {
            Podpisanty pod = new Podpisanty();
            pod = db.Podpisanty.FirstOrDefault(a => a.ID == ID);
            return PartialView(pod);
        }
        //-------------------------------//

        //Сохранение редактирования члена комиссии------------//
        [HttpPost]
        public ActionResult KomissiyaEditSave(int ID, string Nazn, string Doljnost, string FIO)
        {
            try
            {
                Podpisanty pod = new Podpisanty();
                pod = db.Podpisanty.FirstOrDefault(s => s.ID == ID);

                pod.Nazn = Nazn.Trim();
                pod.Doljnost = Doljnost.Trim();
                pod.FIO = FIO.Trim();
                db.SaveChanges();

                ViewBag.Message = "Подписант успешно изменен";

            }
            catch (Exception e)
            {
                ViewBag.Message = "Ошибка. Текст ошибки: " + e.ToString();
            }

            return PartialView();
        }
        //-----------------------------//

        //-----------------------------------------------Ресурсы нефти----------------------------------------------------------------//

        public ActionResult Resursy()
        {
            List<ResursiNefti> Resurs = new List<ResursiNefti>();
            Resurs = db.ResursiNefti.OrderBy(h => h.idFilial).ToList();

            SelectList res = new SelectList(db.filials, "id", "name");
            ViewBag.res = res;

            return View(Resurs);
        }

        public ActionResult ResursyMoz()
        {
            List<ResursiNefti> ResursMoz = new List<ResursiNefti>();
            ResursMoz = db.ResursiNefti.Where(h => h.idFilial == 1).ToList();

            SelectList resMoz = new SelectList(db.filials, "id", "name");
            ViewBag.resMoz = resMoz;

            return View(ResursMoz);
        }

        public ActionResult ResursyNovop()
        {
            List<ResursiNefti> ResursNovop = new List<ResursiNefti>();
            ResursNovop = db.ResursiNefti.Where(h => h.idFilial == 2).ToList();

            SelectList resNovop = new SelectList(db.filials, "id", "name");
            ViewBag.resNovop = resNovop;

            return View(ResursNovop);
        }

        //----------Добавление ресурса хранения нефти-----------------------//

        public ActionResult AddResursy()
        {
            SelectList komis = new SelectList(db.filials, "id", "name");
            ViewBag.komis = komis;
            return PartialView();
        }
        //--------------------------//
        //-----Сохранение добавления ресурса хранения нефти-----------//
        [HttpPost]
        public ActionResult ResursySave(int ResFil, string Naimenovanie, double VRes)
        {
            try
            {
                ResursiNefti res = new ResursiNefti();
                res.idFilial = Convert.ToInt32(ResFil);
                res.Naimenovanie = Naimenovanie.Trim();
                res.VNefti = Convert.ToDecimal(VRes);

                db.ResursiNefti.Add(res);

                db.SaveChanges();

                ViewBag.Message = "Запись успешно добавлена!";
            }
            catch (Exception ex)
            {


                ViewBag.Message = ex.ToString();
            }

            return PartialView();
        }
        //----------------------------------//

        //----------Добавление ресурса хранения нефти ЛПДС Мозырь-----------------------//

        public ActionResult AddResursyMoz()
        {
            return PartialView();
        }
        //--------------------------//
        //-----Сохранение добавления ресурса хранения нефти ЛПДС Мозырь----------//
        [HttpPost]
        public ActionResult ResursySaveMoz(string NaimenovanieM, double VResM)
        {
            try
            {
                ResursiNefti resM = new ResursiNefti();
                resM.idFilial = 1;
                resM.Naimenovanie = NaimenovanieM.Trim();
                resM.VNefti = Convert.ToDecimal(VResM);

                db.ResursiNefti.Add(resM);

                db.SaveChanges();

                ViewBag.Message = "Запись успешно добавлена!";
            }
            catch (Exception ex)
            {


                ViewBag.Message = ex.ToString();
            }

            return PartialView();
        }
        //----------------------------------//

        //----------Добавление ресурса хранения нефти ЛПДС Полоцк-----------------------//

        public ActionResult AddResursyNovop()
        {
            return PartialView();
        }
        //--------------------------//
        //-----Сохранение добавления ресурса хранения нефти ЛПДС Полоцк-----------//
        [HttpPost]
        public ActionResult ResursySaveNovop(string NaimenovanieN, double VResN)
        {
            try
            {
                ResursiNefti resN = new ResursiNefti();
                resN.idFilial = 2;
                resN.Naimenovanie = NaimenovanieN.Trim();
                resN.VNefti = Convert.ToDecimal(VResN);

                db.ResursiNefti.Add(resN);

                db.SaveChanges();

                ViewBag.Message = "Запись успешно добавлена!";
            }
            catch (Exception ex)
            {


                ViewBag.Message = ex.ToString();
            }

            return PartialView();
        }
        //----------------------------------//
        //-------------------------------------
        // удаление ресурса хранения нефти//

        public ActionResult DeleteResursy(int ID)
        {
            ResursiNefti res = new ResursiNefti();
            res = db.ResursiNefti.FirstOrDefault(a => a.id == ID);
            return PartialView(res);
        }
        //-----------------------------//

        // Подтверждение удаления ресурса хранения нефти//
        public ActionResult DeleteResursyOK(int ID)
        {
            try
            {

                ResursiNefti resN = new ResursiNefti();
                resN = db.ResursiNefti.FirstOrDefault(a => a.id == ID);
                db.ResursiNefti.Remove(resN);
                db.SaveChanges();

                ViewBag.Message = "Ресурс нефти успешно удален!";
            }
            catch
            {
                ViewBag.Message = "Ошибка удаления";
            }

            return PartialView();
        }
        // Редактирование ресурса хранения нефти//

        public ActionResult ResursyEdit(int ID)
        {

            ResursiNefti res = new ResursiNefti();
            res = db.ResursiNefti.FirstOrDefault(a => a.id == ID);

            SelectList res1 = new SelectList(db.filials, "id", "name");
            ViewBag.res1 = res1;

            return PartialView(res);

        }
        //-------------------------------//

        //Сохранение редактирования ресурса хранения нефти------------//
        [HttpPost]
        public ActionResult ResursyEditSave(int ID, string Naimen, decimal VResurs)
        {
            try
            {
                ResursiNefti res = new ResursiNefti();
                res = db.ResursiNefti.FirstOrDefault(s => s.id == ID);

                res.Naimenovanie = Naimen.Trim();
                res.VNefti = VResurs;

                db.SaveChanges();

                ViewBag.Message = "Ресурс нефти успешно изменен";

            }
            catch (Exception e)
            {
                ViewBag.Message = "Ошибка. Текст ошибки: " + e.ToString();
            }

            return PartialView();
        }
        //-----------------------------//

        //----------------------------------------------------------------------------------------------------------------------------------------//

        public ActionResult Reference()
        {

            return PartialView();
        }
        //------------------Список резервуаров в зависимости от филиала--------------------
        public ActionResult GetRezerv(int ID)
        {
            //List<string> Rezerv = new List<string>();
            ////Rezerv = db.trl_tankview.Where(b => b.name.Equals(ID)).Select(x => x.name).Distinct().ToList();
            //Rezerv = db.trl_tankview.Where(b => b.FilialID == ID).Select(f => f.TankName).ToList();

            List<trl_tankview> Rezerv = new List<trl_tankview>();
            Rezerv = db.trl_tankview.Where(h => h.FilialID == ID).ToList();

            return PartialView(Rezerv);
        }

        //-----------------Получение градуировочной таблицы в зависимоти от резервуара-----------------------------------
        public ActionResult GetGradTable(int rezer, int filial)
        {
            List<calibration> GetTab = new List<calibration>();
            GetTab = db.calibration.Where(j => j.tankid == rezer & j.filialid == filial).OrderBy(h => h.oillevel).ToList();

            ViewBag.GetTab = GetTab;

            return PartialView(GetTab);
        }

        //----------------------------------//
        // удаление градуировочной таблицы//

        public ActionResult DeleteGrad(int ID)
        {
            calibration calibr = new calibration();
            calibr = db.calibration.FirstOrDefault(a => a.id == ID);
            return PartialView(calibr);
        }
        //-----------------------------//

        // Подтверждение удаления градуировочной таблицы//
        public ActionResult DeleteGradOK(int ID)
        {
            try
            {

                calibration calibr = new calibration();
                calibr = db.calibration.FirstOrDefault(a => a.id == ID);
                db.calibration.Remove(calibr);
                db.SaveChanges();

                ViewBag.Message = "Строка успешно удалена!";
            }
            catch
            {
                ViewBag.Message = "Ошибка удаления";
            }

            return PartialView();
        }
        // Редактирование градуировочной таблицы//

        public ActionResult GradEdit(int ID)
        {
            calibration calibr = new calibration();
            calibr = db.calibration.FirstOrDefault(a => a.id == ID);
            return PartialView(calibr);
        }
        //-------------------------------//

        //Сохранение редактирования градуировочной таблицы------------//
        [HttpPost]
        public ActionResult GradEditSave(int ID, int urov, string V)
        {
            try
            {
                calibration calibr = new calibration();
                calibr = db.calibration.FirstOrDefault(s => s.id == ID);

                calibr.oillevel = urov;
                calibr.oilvolume = Convert.ToDouble(V);
                db.SaveChanges();

                ViewBag.Message = "Строка успешно изменена";

            }
            catch (Exception e)
            {
                ViewBag.Message = "Ошибка. Текст ошибки: " + e.ToString();
            }

            return PartialView();
        }
        //-----------------------------//
        //----------Добавление строки в градуировочную таблицу-----------------------//

        public ActionResult AddGradTable()
        {
            SelectList calibr = new SelectList(db.calibration, "id", "tankid");
            ViewBag.calibr = calibr;

            return PartialView();
        }
        //--------------------------//
        //-----Сохранение добавления строки в градуировочную таблицу-----------//
        [HttpPost]
        public ActionResult GradTableSave(int IDRez, int urov, string V)
        {
            try
            {
                calibration cal = new calibration();
                cal.tankid = IDRez;
                cal.oillevel = urov;
                cal.oilvolume = Convert.ToDouble(V);
                db.calibration.Add(cal);

                db.SaveChanges();

                ViewBag.Message = "Строка успешно добавлендобавлена!";
            }
            catch (Exception ex)
            {


                ViewBag.Message = ex.ToString();
            }

            return PartialView();
        }
        //----------------------------------//

        //---------Получение даты инвентаризации в зависимости от филиала-----------------------------
        public ActionResult GetDatInv(int ID, DateTime Ss, DateTime Popo)
        {

            List<TankInv> DatInv = new List<TankInv>();
            DatInv = db.TankInv.Where(h => h.Filial == ID).ToList();

            SelectList podrazdD1 = new SelectList(db.filials, "id", "name");
            ViewBag.podr1 = podrazdD1;

            List<TankInv> DatINV = new List<TankInv>();
            DatINV = db.TankInv.Where(j => j.Filial == ID && j.Data >= Ss && j.Data <= Popo).OrderByDescending(h => h.Data).ToList();
            List<TankInv> DatINVLast = new List<TankInv>();

            var maxDatG = db.TankInv.Where(k => k.Filial == 1).Max(y => y.Data);
            DatInv = db.TankInv.Where(j => j.Filial == ID & j.Data == maxDatG).OrderBy(h => h.Rezer).ToList();

            SelectList inventGomel = new SelectList(db.filials, "id", "name");
            ViewBag.datinv = DatInv;

            List<DateTime> datSelect = new List<DateTime>();

            foreach (var item in DatINV)
            {
                datSelect.Add(item.Data);
            }
            List<DateTime> datSelectDiST = new List<DateTime>();
            datSelectDiST = datSelect.Distinct().ToList();
            ViewBag.SelDat = datSelectDiST;

            return PartialView();
        }
        //--------------------------------------------------------------------------------------------
        //-------------------Вывод таблицы акта ивентаризации-----------------------------------------
        public ActionResult GetTableInv(DateTime datinv, int filial1)
        {
            //-----------------------------------------------------------------------------------------------------------
            List<TankInv> TableInv = new List<TankInv>();
            TableInv = db.TankInv.Where(j => j.Filial == filial1 & j.Data == datinv).OrderBy(h => h.type).ThenBy(h => h.Rezer).ToList();

            //список тиров резервуаров
            //--------------------------------------------------
            List<trl_tank> trl = new List<trl_tank>();
            trl = db.trl_tank.Where(p => p.FilialID == filial1).OrderBy(p => p.TypeID).ThenBy(p => p.TankID).ToList();

            List<int> typ = new List<int>();
            foreach (var t in trl)
            {
                {
                    if (t.TypeID != 0)
                        typ.Add(t.TypeID);
                }
            }

            List<int> typDyst = new List<int>();

            typDyst = typ.Distinct().ToList();
            ViewBag.typ = typDyst;

            //--------------------------------------------
            int kol = 0;
            double VNeftItog1 = 0;
            double PSred1 = 0;
            double MassaBalItog1 = 0;
            double BalProcSred1 = 0;
            double BalTonnItog1 = 0;
            double MassaNettoItog1 = 0;
            double MassaBalMinItog1 = 0;
            double MassaNettoMinItog1 = 0;
            double MassaBruttoOstat1 = 0;
            double VH2OItog1 = 0;

            foreach (var t in TableInv)
            {
                kol++;
                VNeftItog1 = VNeftItog1 + Convert.ToDouble(t.VNeft);
                PSred1 = Convert.ToDouble(t.P) + PSred1;
                MassaBalItog1 = MassaBalItog1 + Convert.ToDouble(t.MassaBrutto);
                BalProcSred1 = BalProcSred1 + Convert.ToDouble(t.BalProc);
                BalTonnItog1 = BalTonnItog1 + Convert.ToDouble(t.BalTonn);
                MassaNettoItog1 = MassaNettoItog1 + Convert.ToDouble(t.MassaNetto);
                MassaBalMinItog1 = MassaBalMinItog1 + Convert.ToDouble(t.MBalMin);
                MassaNettoMinItog1 = MassaNettoMinItog1 + Convert.ToDouble(t.MNettoMin);
                MassaBruttoOstat1 = MassaBruttoOstat1 + Convert.ToDouble(t.MBalMin);
                VH2OItog1 = VH2OItog1 + Convert.ToDouble(t.VH2O);
            }
            //---------------------------------------------
            ViewBag.VNeftItog1 = VNeftItog1;
            ViewBag.PSred1 = Math.Round(MassaBalItog1 / VNeftItog1, 1);
            ViewBag.MassaBalItog1 = Math.Round(MassaBalItog1, 1);
            ViewBag.BalProcSred1 = Math.Round(BalProcSred1 / kol, 1);
            ViewBag.BalTonnItog1 = Math.Round(BalTonnItog1, 1);
            ViewBag.MassaNettoItog1 = Math.Round(MassaNettoItog1, 1);
            ViewBag.MassaBalMinItog1 = Math.Round(MassaBalMinItog1, 1);
            ViewBag.MassaNettoMinItog1 = Math.Round(MassaNettoMinItog1, 1);
            ViewBag.MassaBruttoOstat1 = Math.Round(MassaBruttoOstat1, 1);
            ViewBag.VH2OItog1 = Math.Round(VH2OItog1, 1);

            ViewBag.TableInv = TableInv;

            return PartialView(TableInv);
            //------------------------------------------------------------------------------------------------------------
        }
        //--------------------------------------------------------------------------------------------               

        //----------Получить химанализ в зависимости от резервуара-------------------------------
        public ActionResult GetHim(int filial, int rezer)
        {

            //-----------------------------------------------

            //----------------------------------------------


            HIM H = new HIM();
            if (filial == 1)
            {
                LastTanksResultMoz MOZ = new LastTanksResultMoz();

                MOZ = db2.LastTanksResultMoz.FirstOrDefault(f => f.tankid == rezer);

                if (MOZ == null)
                {
                    H.dat = null;
                    H.densreal = null;
                    H.dens20 = null;
                    H.tempreal = null;
                    H.water = null;
                    H.saltmg = null;
                    H.mechan = null;

                }
                else
                {

                    H.dat = MOZ.endsampledt;
                    H.densreal = MOZ.densreal;
                    H.dens20 = MOZ.dens20;
                    H.tempreal = Convert.ToDouble(MOZ.tempreal);
                    H.water = MOZ.water;
                    H.saltmg = MOZ.saltmg;
                    H.mechan = db2.LastMechanResultMoz.FirstOrDefault(g => g.tankid == rezer).mechan.Value; ;
                }


            }
            if (filial == 2)
            {
                LastTanksResultPol MOZ = new LastTanksResultPol();
                MOZ = db2.LastTanksResultPol.FirstOrDefault(j => j.tankid == rezer);
                if (MOZ == null)
                {
                    H.dat = null;
                    H.densreal = null;
                    H.dens20 = null;
                    H.tempreal = null;
                    H.water = null;
                    H.saltmg = null;
                    H.mechan = null;

                }
                else
                {
                    H.dat = MOZ.sampledt;
                    H.densreal = MOZ.densreal;
                    H.dens20 = MOZ.dens20;
                    H.tempreal = Convert.ToDouble(MOZ.tempreal);
                    H.water = MOZ.water;
                    H.saltmg = MOZ.saltmg;
                    H.mechan = 0;
                }

            }
            ViewBag.dat = H.dat;
            ViewBag.dens = H.densreal;
            ViewBag.temp = H.tempreal;
            ViewBag.dens20 = H.dens20;

            double? BallastProc = H.water + H.mechan + H.saltmg / H.densreal / 10;
            ViewBag.BallastProc = Math.Round(Convert.ToDouble(BallastProc), 4);
            return PartialView();
        }

        //----------Калькулятор разность объемов-------------------------------
        public ActionResult GetRazn(int filial1, int rezer1, string Unach, string Ukon, string UnachH2O, string UkonH2O, string Pnach, string Pkon, string Tnach, string Tkon, string Bal, string Kst, string Ksr)
        {
            HIM H = new HIM();
            double V1;
            double V1R;
            double V1RN;
            double V2;
            double V2R;
            double V2RN;
            double RV;
            double RM;
            double Unachmin;
            double Unachmax;
            double Vnachmin;
            double Vnachmax;
            double Upercentnach;
            double Mnach;
            double RMB;

            //Для начальной подтоварной воды
            double UnachminH2O;
            double UnachmaxH2O;
            double VnachminH2O;
            double VnachmaxH2O;
            double V1H2O;
            double V1RH2O;
            double UpercentnachH2O;
            //--------------------------------

            double Ukonmin;
            double Ukonmax;
            double Vkonmin;
            double Vkonmax;
            double Upercentkon;
            double Mkon;

            //Для подтоварной воды конечной
            double UkonminH2O;
            double UkonmaxH2O;
            double VkonminH2O;
            double VkonmaxH2O;
            double V2H2O;
            double V2RH2O;
            double UpercentkonH2O;
            //--------------------------------

            List<calibration> calibrationList = new List<calibration>();
            calibrationList = db.calibration.Where(f => f.filialid == filial1).OrderBy(f => f.tankid).ThenBy(f => f.oillevel).ToList();

            if (filial1 == 1)
            {
                LastTanksResultMoz MOZ = new LastTanksResultMoz();
                MOZ = db2.LastTanksResultMoz.FirstOrDefault(f => f.tankid == rezer1);
                if (MOZ == null)
                {
                    H.dat = null;
                    H.densreal = null;
                    H.dens20 = null;
                    H.tempreal = null;
                    H.water = null;
                    H.saltmg = null;
                    H.mechan = null;
                }
                else
                {
                    H.dat = MOZ.endsampledt;
                    H.densreal = MOZ.densreal;
                    H.dens20 = MOZ.dens20;
                    H.tempreal = Convert.ToDouble(MOZ.tempreal);
                    H.water = MOZ.water;
                    H.saltmg = MOZ.saltmg;
                    H.mechan = MOZ.mechan;
                }
            }

            if (filial1 == 2)
            {
                LastTanksResultPol MOZ = new LastTanksResultPol();
                MOZ = db2.LastTanksResultPol.FirstOrDefault(j => j.tankid == rezer1);
                if (MOZ == null)
                {
                    H.dat = null;
                    H.densreal = null;
                    H.dens20 = null;
                    H.tempreal = null;
                    H.water = null;
                    H.saltmg = null;
                    H.mechan = null;
                }
                else
                {
                    H.dat = MOZ.sampledt;
                    H.densreal = MOZ.densreal;
                    H.dens20 = MOZ.dens20;
                    H.tempreal = Convert.ToDouble(MOZ.tempreal);
                    H.water = MOZ.water;
                    H.saltmg = MOZ.saltmg;
                    H.mechan = MOZ.mechan;
                }
            }

            if (calibrationList.Where(h => h.tankid == rezer1).Count() == 0)
            {
                ViewBag.V1 = "данных нет!";
                ViewBag.V2 = "даных нет!";
                ViewBag.Mnach = "данных нет!";
                ViewBag.Mkon = "данных нет!";
                ViewBag.RV = "данных нет!";
                ViewBag.RM = "данных нет!";
            }
            else if (calibrationList.LastOrDefault(g => g.tankid == rezer1).oillevel < Convert.ToDouble(Unach) | calibrationList.LastOrDefault(g => g.tankid == rezer1).oillevel < Convert.ToDouble(Ukon))
            {
                ViewBag.V1 = "Большое значение!";
                ViewBag.V2 = "Большое значение!";
                ViewBag.Mnach = "Большое значение!";
                ViewBag.Mkon = "Большое значение!";
                ViewBag.RV = "Большое значение!";
                ViewBag.RM = "Большое значение!";
            }
            else
            {

                //-------Рассчет объема нефти начального---------------------------------------------------------------------------
                if (calibrationList.LastOrDefault(g => g.tankid == rezer1 & g.oillevel <= Convert.ToDouble(Unach)) == null)
                {
                    Unachmin = 0;
                }
                else
                {
                    Unachmin = calibrationList.LastOrDefault(g => g.tankid == rezer1 & g.oillevel <= Convert.ToDouble(Unach)).oillevel;
                }
                Unachmax = calibrationList.FirstOrDefault(g => g.tankid == rezer1 & g.oillevel > Convert.ToDouble(Unach)).oillevel;
                if (calibrationList.LastOrDefault(g => g.tankid == rezer1 & g.oillevel <= Convert.ToDouble(Unach)) == null)
                {
                    Vnachmin = 0;
                }
                else
                {
                    Vnachmin = calibrationList.LastOrDefault(g => g.tankid == rezer1 & g.oillevel <= Convert.ToDouble(Unach)).oilvolume;
                }
                Vnachmax = calibrationList.FirstOrDefault(g => g.tankid == rezer1 & g.oillevel > Convert.ToDouble(Unach)).oilvolume;
                Upercentnach = (Convert.ToDouble(Unach) - Unachmin) / (Unachmax - Unachmin);
                V1 = Vnachmin + (Vnachmax - Vnachmin) * Upercentnach; //по градуировочной таблице
                V1R = V1 * (1 + (2 * Convert.ToDouble(Kst) + Convert.ToDouble(Ksr)) * (Convert.ToDouble(Tnach) - 20)); //рассчитано по формуле

                //---------Рассчет объема подтоварной воды начальной-------------------------------------------------------
                if (calibrationList.LastOrDefault(g => g.tankid == rezer1 & g.oillevel <= Convert.ToDouble(UnachH2O)) == null)
                {
                    UnachminH2O = 0;
                }
                else
                {
                    UnachminH2O = calibrationList.LastOrDefault(g => g.tankid == rezer1 & g.oillevel <= Convert.ToDouble(UnachH2O)).oillevel;
                }
                UnachmaxH2O = calibrationList.FirstOrDefault(g => g.tankid == rezer1 & g.oillevel > Convert.ToDouble(UnachH2O)).oillevel;
                if (calibrationList.LastOrDefault(g => g.tankid == rezer1 & g.oillevel <= Convert.ToDouble(UnachH2O)) == null | Convert.ToDouble(UnachH2O) == 0)
                {
                    VnachminH2O = 0;
                }
                else
                {
                    VnachminH2O = calibrationList.LastOrDefault(g => g.tankid == rezer1 & g.oillevel <= Convert.ToDouble(UnachH2O)).oilvolume;
                }
                VnachmaxH2O = calibrationList.FirstOrDefault(g => g.tankid == rezer1 & g.oillevel > Convert.ToDouble(UnachH2O)).oilvolume;
                UpercentnachH2O = (Convert.ToDouble(UnachH2O) - UnachminH2O) / (UnachmaxH2O - UnachminH2O);
                V1H2O = VnachminH2O + (VnachmaxH2O - VnachminH2O) * UpercentnachH2O;
                V1RH2O = V1H2O * (1 + (Convert.ToDouble(Kst) + Convert.ToDouble(Ksr)) * (Convert.ToDouble(Tnach) - 20));

                V1RN = (Math.Round(V1, 3) - Math.Round(V1H2O, 3)) * (1 + (2 * Convert.ToDouble(Kst) + Convert.ToDouble(Ksr)) * (Convert.ToDouble(Tnach) - 20)); // Рассчет конечного объема чистой нефти без подтоварной воды

                //---------Рассчет объема нефти конечного----------------------------------------------------------------------------
                if (calibrationList.LastOrDefault(g => g.tankid == rezer1 & g.oillevel <= Convert.ToDouble(Ukon)) == null)
                {
                    Ukonmin = 0;
                }
                else
                {
                    Ukonmin = calibrationList.LastOrDefault(g => g.tankid == rezer1 & g.oillevel <= Convert.ToDouble(Ukon)).oillevel;
                }
                Ukonmax = calibrationList.FirstOrDefault(g => g.tankid == rezer1 & g.oillevel > Convert.ToDouble(Ukon)).oillevel;
                if (calibrationList.LastOrDefault(g => g.tankid == rezer1 & g.oillevel <= Convert.ToDouble(Ukon)) == null)
                {
                    Vkonmin = 0;
                }
                else
                {
                    Vkonmin = calibrationList.LastOrDefault(g => g.tankid == rezer1 & g.oillevel <= Convert.ToDouble(Ukon)).oilvolume;
                }
                Vkonmax = calibrationList.FirstOrDefault(g => g.tankid == rezer1 & g.oillevel > Convert.ToDouble(Ukon)).oilvolume;
                Upercentkon = (Convert.ToDouble(Ukon) - Ukonmin) / (Ukonmax - Ukonmin);
                V2 = Vkonmin + (Vkonmax - Vkonmin) * Upercentkon;
                V2R = V2 * (1 + (Convert.ToDouble(Kst) + Convert.ToDouble(Ksr)) * (Convert.ToDouble(Tkon) - 20));

                //------------Рассчет объема подтоварной воды конечной-----------------------------------------------------------------------
                if (calibrationList.LastOrDefault(g => g.tankid == rezer1 & g.oillevel <= Convert.ToDouble(UkonH2O)) == null)
                {
                    UkonminH2O = 0;
                }
                else
                {
                    UkonminH2O = calibrationList.LastOrDefault(g => g.tankid == rezer1 & g.oillevel <= Convert.ToDouble(UkonH2O)).oillevel;
                }
                UkonmaxH2O = calibrationList.FirstOrDefault(g => g.tankid == rezer1 & g.oillevel > Convert.ToDouble(UkonH2O)).oillevel;
                if (calibrationList.LastOrDefault(g => g.tankid == rezer1 & g.oillevel <= Convert.ToDouble(UkonH2O)) == null | Convert.ToDouble(UkonH2O) == 0)
                {
                    VkonminH2O = 0;
                }
                else
                {
                    VkonminH2O = calibrationList.LastOrDefault(g => g.tankid == rezer1 & g.oillevel <= Convert.ToDouble(UkonH2O)).oilvolume;
                }
                VkonmaxH2O = calibrationList.FirstOrDefault(g => g.tankid == rezer1 & g.oillevel > Convert.ToDouble(UkonH2O)).oilvolume;
                UpercentkonH2O = (Convert.ToDouble(UkonH2O) - UkonminH2O) / (UkonmaxH2O - UkonminH2O);
                V2H2O = VkonminH2O + (VkonmaxH2O - VkonminH2O) * UpercentkonH2O;
                V2RH2O = V2H2O * (1 + (Convert.ToDouble(Kst) + Convert.ToDouble(Ksr)) * (Convert.ToDouble(Tkon) - 20));

                V2RN = (Math.Round(V2, 3) - Math.Round(V2H2O, 3)) * (1 + (2 * Convert.ToDouble(Kst) + Convert.ToDouble(Ksr)) * (Convert.ToDouble(Tkon) - 20)); //Расчет конечного объема читой нефти без подтоварной воды
                                                                                                                                                               //--------------------------------------------------------------------------------------------------------------------
                Mnach = Convert.ToDouble(Pnach) * Math.Round(V1RN, 3) / 1000;
                Mkon = Convert.ToDouble(Pkon) * Math.Round(V2RN, 3) / 1000;
                RV = Math.Round(V2RN, 3) - Math.Round(V1RN, 3);
                RM = Mkon - Mnach;
                RMB = RM - RM * Convert.ToDouble(Bal) / 100;


                ViewBag.V1 = Math.Round(V1RN, 3);
                ViewBag.V2 = Math.Round(V2RN, 3);
                ViewBag.Mnach = Math.Round(Mnach, 3);
                ViewBag.Mkon = Math.Round(Mkon, 3);
                ViewBag.RV = Math.Round(RV, 0);
                ViewBag.RM = Math.Round(RM, 0);
                ViewBag.RMB = Math.Round(RMB, 0);
            }
            return PartialView();
        }

        public ActionResult GetRezerv1(int ID)
        {
            List<trl_tankview> Rezerv1 = new List<trl_tankview>();
            Rezerv1 = db.trl_tankview.Where(h => h.FilialID == ID).ToList();

            return PartialView(Rezerv1);
        }

        public ActionResult GetRezerv2(int ID)
        {
            List<trl_tankview> Rezerv2 = new List<trl_tankview>();
            Rezerv2 = db.trl_tankview.Where(h => h.FilialID == ID).ToList();

            return PartialView(Rezerv2);
        }

        //----------Калькулятор разность объемов по массе-------------------------------
        public ActionResult GetRazn1(int filial2, int rezer2, string Unach1, string RM1, string UnachH2O1, string UkonH2O1, string Pnach1, string Pkon1, string Tnach1, string Tkon1, string Bal1, string Kst1, string Ksr1)
        {
            HIM H = new HIM();
            double V1;
            double V1R;
            double V1RN;
            double V2;
            double V2R;
            double V2RN;
            double RV;
            double RM;
            double Unachmin;
            double Unachmax;
            double Vnachmin;
            double Vnachmax;
            double Upercentnach;
            double Mnach1;
            double RMB;

            //Для начальной подтоварной воды
            double UnachminH2O;
            double UnachmaxH2O;
            double VnachminH2O;
            double VnachmaxH2O;
            double V1H2O;
            double V1RH2O;
            double UpercentnachH2O;
            //--------------------------------

            double Ukonmin;
            double Ukonmax;
            double Vkonmin;
            double Vkonmax;
            double Upercentkon;
            double Mkon1;
            double Ukon1;

            //Для подтоварной воды конечной
            double UkonminH2O;
            double UkonmaxH2O;
            double VkonminH2O;
            double VkonmaxH2O;
            double V2H2O;
            double V2RH2O;
            double UpercentkonH2O;
            //--------------------------------

            List<calibration> calibrationList = new List<calibration>();
            calibrationList = db.calibration.Where(f => f.filialid == filial2 && f.tankid == rezer2).OrderBy(h => h.oillevel).ToList();

            if (filial2 == 1)
            {
                LastTanksResultMoz MOZ = new LastTanksResultMoz();
                MOZ = db2.LastTanksResultMoz.FirstOrDefault(f => f.tankid == rezer2);
                if (MOZ == null)
                {
                    H.dat = null;
                    H.densreal = null;
                    H.dens20 = null;
                    H.tempreal = null;
                    H.water = null;
                    H.saltmg = null;
                    H.mechan = null;
                }
                else
                {
                    H.dat = MOZ.endsampledt;
                    H.densreal = MOZ.densreal;
                    H.dens20 = MOZ.dens20;
                    H.tempreal = Convert.ToDouble(MOZ.tempreal);
                    H.water = MOZ.water;
                    H.saltmg = MOZ.saltmg;
                    H.mechan = MOZ.mechan;
                }
            }

            if (filial2 == 2)
            {
                LastTanksResultPol MOZ = new LastTanksResultPol();
                MOZ = db2.LastTanksResultPol.FirstOrDefault(j => j.tankid == rezer2);
                if (MOZ == null)
                {
                    H.dat = null;
                    H.densreal = null;
                    H.dens20 = null;
                    H.tempreal = null;
                    H.water = null;
                    H.saltmg = null;
                    H.mechan = null;
                }
                else
                {
                    H.dat = MOZ.sampledt;
                    H.densreal = MOZ.densreal;
                    H.dens20 = MOZ.dens20;
                    H.tempreal = Convert.ToDouble(MOZ.tempreal);
                    H.water = MOZ.water;
                    H.saltmg = MOZ.saltmg;
                    H.mechan = MOZ.mechan;
                }
            }

            if (calibrationList.Where(h => h.tankid == rezer2).Count() == 0)
            {
                ViewBag.V1 = "данных нет!";
                ViewBag.V2 = "даных нет!";
                ViewBag.Mnach = "данных нет!";
                ViewBag.Mkon = "данных нет!";
                ViewBag.RV = "данных нет!";
                ViewBag.RM = "данных нет!";
            }
            else if (calibrationList.LastOrDefault(g => g.tankid == rezer2).oillevel < Convert.ToDouble(Unach1))
            {
                ViewBag.V1 = "Большое значение!";
                ViewBag.V2 = "Большое значение!";
                ViewBag.Mnach1 = "Большое значение!";
                ViewBag.Mkon1 = "Большое значение!";
                ViewBag.RV1 = "Большое значение!";
                ViewBag.Ukon1 = "Большое значение!";
                ViewBag.MNet = "Большое значение!";
            }
            else
            {
                //-------Рассчет объема нефти начального---------------------------------------------------------------------------
                if (calibrationList.LastOrDefault(g => g.tankid == rezer2 & g.oillevel <= Convert.ToDouble(Unach1)) == null)
                {
                    Unachmin = 0;
                }
                else
                {
                    Unachmin = calibrationList.LastOrDefault(g => g.tankid == rezer2 & g.oillevel <= Convert.ToDouble(Unach1)).oillevel;
                }
                Unachmax = calibrationList.FirstOrDefault(g => g.tankid == rezer2 & g.oillevel > Convert.ToDouble(Unach1)).oillevel;
                if (calibrationList.LastOrDefault(g => g.tankid == rezer2 & g.oillevel <= Convert.ToDouble(Unach1)) == null)
                {
                    Vnachmin = 0;
                }
                else
                {
                    Vnachmin = calibrationList.LastOrDefault(g => g.tankid == rezer2 & g.oillevel <= Convert.ToDouble(Unach1)).oilvolume;
                }
                Vnachmax = calibrationList.FirstOrDefault(g => g.tankid == rezer2 & g.oillevel > Convert.ToDouble(Unach1)).oilvolume;
                Upercentnach = (Convert.ToDouble(Unach1) - Unachmin) / (Unachmax - Unachmin);
                V1 = Vnachmin + (Vnachmax - Vnachmin) * Upercentnach; //по градуировочной таблице
                                                                      //V1R = V1 * (1 + (2 * Convert.ToDouble(Kst1) + Convert.ToDouble(Ksr1)) * (Convert.ToDouble(Tnach1) - 20)); //рассчитано по формуле

                //---------Рассчет объема подтоварной воды начальной-------------------------------------------------------
                if (calibrationList.LastOrDefault(g => g.tankid == rezer2 & g.oillevel <= Convert.ToDouble(UnachH2O1)) == null)
                {
                    UnachminH2O = 0;
                }
                else
                {
                    UnachminH2O = calibrationList.LastOrDefault(g => g.tankid == rezer2 & g.oillevel <= Convert.ToDouble(UnachH2O1)).oillevel;
                }
                UnachmaxH2O = calibrationList.FirstOrDefault(g => g.tankid == rezer2 & g.oillevel > Convert.ToDouble(UnachH2O1)).oillevel;
                if (calibrationList.LastOrDefault(g => g.tankid == rezer2 & g.oillevel <= Convert.ToDouble(UnachH2O1)) == null | Convert.ToDouble(UnachH2O1) == 0)
                {
                    VnachminH2O = 0;
                }
                else
                {
                    VnachminH2O = calibrationList.LastOrDefault(g => g.tankid == rezer2 & g.oillevel <= Convert.ToDouble(UnachH2O1)).oilvolume;
                }
                VnachmaxH2O = calibrationList.FirstOrDefault(g => g.tankid == rezer2 & g.oillevel > Convert.ToDouble(UnachH2O1)).oilvolume;
                UpercentnachH2O = (Convert.ToDouble(UnachH2O1) - UnachminH2O) / (UnachmaxH2O - UnachminH2O);
                V1H2O = VnachminH2O + (VnachmaxH2O - VnachminH2O) * UpercentnachH2O;
                //V1RH2O = V1H2O * (1 + (2 * Convert.ToDouble(Kst1) + Convert.ToDouble(Ksr1)) * (Convert.ToDouble(Tnach1) - 20));

                V1RN = (Math.Round(V1, 3) - Math.Round(V1H2O, 3)) * (1 + (2 * Convert.ToDouble(Kst1) + Convert.ToDouble(Ksr1)) * (Convert.ToDouble(Tnach1) - 20)); // Рассчет начального объема чистой нефти без подтоварной воды

                //------------Рассчет объема подтоварной воды конечной-----------------------------------------------------------------------
                if (calibrationList.LastOrDefault(g => g.tankid == rezer2 & g.oillevel <= Convert.ToDouble(UkonH2O1)) == null)
                {
                    UkonminH2O = 0;
                }
                else
                {
                    UkonminH2O = calibrationList.LastOrDefault(g => g.tankid == rezer2 & g.oillevel <= Convert.ToDouble(UkonH2O1)).oillevel;
                }
                UkonmaxH2O = calibrationList.FirstOrDefault(g => g.tankid == rezer2 & g.oillevel > Convert.ToDouble(UkonH2O1)).oillevel;
                if (calibrationList.LastOrDefault(g => g.tankid == rezer2 & g.oillevel <= Convert.ToDouble(UkonH2O1)) == null | Convert.ToDouble(UkonH2O1) == 0)
                {
                    VkonminH2O = 0;
                }
                else
                {
                    VkonminH2O = calibrationList.LastOrDefault(g => g.tankid == rezer2 & g.oillevel <= Convert.ToDouble(UkonH2O1)).oilvolume;
                }
                VkonmaxH2O = calibrationList.FirstOrDefault(g => g.tankid == rezer2 & g.oillevel > Convert.ToDouble(UkonH2O1)).oilvolume;
                UpercentkonH2O = (Convert.ToDouble(UkonH2O1) - UkonminH2O) / (UkonmaxH2O - UkonminH2O);
                V2H2O = VkonminH2O + (VkonmaxH2O - VkonminH2O) * UpercentkonH2O;
                V2RH2O = V2H2O * (1 + (Convert.ToDouble(Kst1) + Convert.ToDouble(Ksr1)) * (Convert.ToDouble(Tkon1) - 20));

                //V2RN = (Math.Round(V2, 3) - Math.Round(V2H2O, 3)) * (1 + (2 * Convert.ToDouble(Kst) + Convert.ToDouble(Ksr)) * (Convert.ToDouble(Tkon) - 20)); //Расчет конечного объема читой нефти без подтоварной воды

                //--------------------------------------------------------------------------------------------------------------------
                //V1RN = (Math.Round(V1, 3) - Math.Round(V1H2O, 3)) * (1 + (2 * Convert.ToDouble(Kst1) + Convert.ToDouble(Ksr1)) * (Convert.ToDouble(Tnach1) - 20)); // Рассчет конечного объема нефти без подтоварной воды

                Mnach1 = Convert.ToDouble(Pnach1) * Math.Round(V1RN, 3) / 1000; // Масса нефти начальная

                RMB = 100 * Convert.ToDouble(RM1) / (100 - Convert.ToDouble(Bal1)); //Рассчет массы(брутто) нефти

                Mkon1 = Convert.ToDouble(RMB) + Math.Round(Mnach1, 3); //Рассчет массы конечной

                V2RN = 1000 * Mkon1 / Convert.ToDouble(Pkon1); //Рассчет объема нефти чистой конечной

                V2 = V2RN / (1 + (2 * Convert.ToDouble(Kst1) + Convert.ToDouble(Ksr1)) * (Convert.ToDouble(Tkon1) - 20)) + V2H2O;

                if (calibrationList.LastOrDefault(g => g.tankid == rezer2 & g.oilvolume <= Convert.ToDouble(V2)) == null | calibrationList.FirstOrDefault(g => g.tankid == rezer2 & g.oilvolume > Convert.ToDouble(V2)) == null)
                {
                    ViewBag.V1 = "Большое значение!";
                    ViewBag.V2 = "Большое значение!";
                    ViewBag.Mnach1 = "Большое значение!";
                    ViewBag.Mkon1 = "Большое значение!";
                    ViewBag.RV1 = "Большое значение!";
                    ViewBag.Ukon1 = "Большое значение!";
                    ViewBag.MNet = "Большое значение!";
                }
                else
                {
                    Vkonmin = calibrationList.LastOrDefault(g => g.oilvolume <= Convert.ToDouble(V2)).oilvolume;
                    Vkonmax = calibrationList.FirstOrDefault(g => g.oilvolume > Convert.ToDouble(V2)).oilvolume;
                    Upercentkon = (V2 - Vkonmin) / (Vkonmax - Vkonmin);
                                      

                    Ukonmin = calibrationList.LastOrDefault(g => g.tankid == rezer2 & g.oilvolume <= Convert.ToDouble(V2)).oillevel;
                    Ukonmax = calibrationList.FirstOrDefault(g => g.tankid == rezer2 & g.oilvolume > Convert.ToDouble(V2)).oillevel;
                    Ukon1 = Ukonmin + (Ukonmax - Ukonmin) * Upercentkon;

                    //Mkon1 = Convert.ToDouble(RM1) + Mnach1;
                    //V2 = Mkon1 / Convert.ToDouble(Pkon1) * 1000;
                    //V2R = V2 * (1 + 0.0000375 * (Convert.ToDouble(Tkon1) - 20));
                    RV = V2RN - V1RN;

                    //Vkonmin = calibrationList.LastOrDefault(g => g.tankid == rezer2 & g.oilvolume <= Convert.ToDouble(V2R)).oilvolume;
                    //Vkonmax = calibrationList.FirstOrDefault(g => g.tankid == rezer2 & g.oilvolume > Convert.ToDouble(V2R)).oilvolume;
                    //Ukonmin = calibrationList.LastOrDefault(g => g.tankid == rezer2 & g.oilvolume <= Convert.ToDouble(V2R)).oillevel;
                    //Ukonmax = calibrationList.FirstOrDefault(g => g.tankid == rezer2 & g.oilvolume > Convert.ToDouble(V2R)).oillevel;
                    //Upercentkon = (V2R - Vkonmin) / (Vkonmax - Vkonmin);
                    //Ukon1 = Ukonmin + (Ukonmax - Ukonmin) * Upercentkon; 


                    ViewBag.V1 = Math.Round(V1RN, 3);
                    ViewBag.V2 = Math.Round(V2RN, 3);
                    ViewBag.Mnach1 = Math.Round(Mnach1, 3);
                    ViewBag.Mkon1 = Math.Round(Mkon1, 3);
                    ViewBag.RV1 = Math.Round(RV, 0);
                    //ViewBag.RM = RM;
                    ViewBag.Ukon1 = Math.Round(Ukon1, 0);
                    ViewBag.MNet = Math.Round((Convert.ToDouble(RM1) + (Convert.ToDouble(RM1) * Convert.ToDouble(Bal1) / 100)), 0);
                }
            }
            return PartialView();
        }
        //-----Формирование отчета в EXCEL----------------------------------------------------------------------

        public ActionResult Export(int filial1, string datinv)
        {
            DateTime datinvAct = Convert.ToDateTime(datinv);

            List<TankInv> Vod = new List<TankInv>();
            Vod = db.TankInv.Where(p => p.Filial == filial1).Where(f => f.Data == datinvAct).OrderBy(g => g.type).ThenBy(g => g.Rezer).ToList();
            var VodGr = Vod.GroupBy(h => h.type); //список сгруппированный по полю type

            List<trl_tank> trl = new List<trl_tank>();
            trl = db.trl_tank.Where(p => p.FilialID == filial1).OrderBy(p => p.TypeID).ThenBy(p => p.TankID).ToList();

            //Заполнение списка ресурсов нефти----------------------------------------------

            List<ResursiNefti> Resursy = new List<ResursiNefti>();
            Resursy = db.ResursiNefti.Where(f => f.idFilial == filial1).ToList();
            //-------------------------------------------------------------------------------

            List<int> typ = new List<int>();
            foreach (var t in trl)
            {
                {
                    typ.Add(t.TypeID);
                }
            }

            List<int> typDyst = new List<int>();

            //---Список типов резервуаров---------------------
            typDyst = typ.Distinct().ToList();
            //-------------------------------------------------

            TankInfoType TanType = new TankInfoType();
            foreach (var p in Vod)
            {
                TanType.tankid = p.Rezer;
                TanType.date = p.Data;
                TanType.filial = p.Filial;
                TanType.level = Convert.ToDouble(p.Urov);
                TanType.temp = Convert.ToDouble(p.Temp);
                if (trl.FirstOrDefault(g => g.TankID == p.Rezer) == null)
                {
                    TanType.type = 777;
                }
                else
                {
                    TanType.type = trl.FirstOrDefault(g => g.TankID == p.Rezer).TypeID;
                }

            }
            string MOL = "";
            List<Podpisanty> Podpis = new List<Podpisanty>();
            Podpis = db.Podpisanty.Where(f => f.IDFilial == filial1 & f.Nazn.Trim() == "Член комиссии").ToList();

            Podpisanty Predsed = db.Podpisanty.Where(g => g.IDFilial == filial1).FirstOrDefault(g => g.Nazn == "Председатель комиссии");
            if (db.Podpisanty.FirstOrDefault(g => g.Nazn == "МОЛ" & g.IDFilial == filial1) == null)
            {
                MOL = "";
            }
            else
            {
                MOL = "Материльно ответственное лицо _______________ " + db.Podpisanty.FirstOrDefault(g => g.IDFilial == filial1 & g.Nazn.Trim() == "МОЛ").FIO;
            }

            //---Задаем параметры листа------------------------------------------------------------

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Лист1");
            worksheet.PageSetup.PaperSize = XLPaperSize.A4Paper;

            //Задаю узкие поля акта при печати, в рот они ебись, чтобы все влезло большими буквами и цифрами

            worksheet.PageSetup.Margins.Top = 0.1;
            worksheet.PageSetup.Margins.Bottom = 0.1;
            worksheet.PageSetup.Margins.Right = 0.1;
            worksheet.PageSetup.Margins.Left = 0.1;
            //---------------------------------------------------------------------------------------------

            worksheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
            worksheet.PageSetup.FitToPages(1, 1);
            worksheet.Style.Font.FontName = "Times New Roman";


            worksheet.Cell("J" + 1).Value = "Форма утверждена Положением по бухгалтерскому и налоговому учету \"Учетная политика ОАО \"Гомельтранснефть дружба\"";
            worksheet.Cell("J" + 1).Style.Font.FontSize = 9;
            //Шапка отчета//
            //worksheet.Columns().AdjustToContents();
            string chleny = "";

            if (filial1 == 1)
            {
                worksheet.Cell("G" + 2).Value = "Акт инвентаризации нефти в резервуарах филиала \"ЛПДС \"Мозырь\"";
            }
            else
            {
                worksheet.Cell("G" + 2).Value = "Акт инвентаризации нефти в резервуарах филиала ЛПДС \"Новополоцк\"";
            }

            string predsFIO;
            string PredsDolj;

            if (Predsed == null)
            {
                predsFIO = "";
                PredsDolj = "";
            }
            else
            {
                predsFIO = Predsed.FIO.Trim();
                PredsDolj = Predsed.Doljnost.Trim();
            }

            worksheet.Cell("A" + 3).Value = "Председатель комиссии - " + PredsDolj + " " + predsFIO;
            foreach (var i in Podpis)
            {
                chleny = chleny + i.Doljnost.Trim() + " " + i.FIO.Trim() + ", ";
            }

            //вывод списка членов комиссии вверху листа

            worksheet.Range("A4:T4").Row(1).Merge();
            worksheet.Range("A4:T4").Style.Alignment.WrapText = true;
            worksheet.Cell("A" + 4).Value = "Члены комиссии - " + chleny;
            worksheet.Row(4).Height = 30;

            //вывод в акте инвентаризациия (.xls), чтобы печаталось 24:00 если выбирают 00:00 и дата - 1 день. Так просили я так сделал.
            //
            //if (Convert.ToDateTime(datinv).ToShortTimeString() == "00:00")
            string s = datinvAct.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss").Substring(11, 3);
                        
            if (s == "00:")
            {
                worksheet.Cell("A" + 5).Value = "составили настоящий акт в том, что по состоянию на 24:00 часа московского времени " + Convert.ToDateTime(datinv).AddDays(-1).ToShortDateString() + " было установлено наличие нефти следующего количества";
            }
            else
            {
                worksheet.Cell("A" + 5).Value = "составили настоящий акт в том, что по состоянию на " + Convert.ToDateTime(datinv).ToShortTimeString() + " часа московского времени " + Convert.ToDateTime(datinv).ToShortDateString() + " было установлено наличие нефти следующего количества";
            }

            worksheet.Cell("E" + 2).Style.Font.FontSize = 12;

            //вывод текста вверху (подписантов и прочую херню шрифтом 12)

            worksheet.Cell("A" + 3).Style.Font.FontSize = 12;
            worksheet.Cell("A" + 4).Style.Font.FontSize = 12;
            worksheet.Cell("A" + 5).Style.Font.FontSize = 12;
            //---------------------------------------------------------

            //вывод шапки таблицы шрифтом 12

            worksheet.Range("A7:I7").Style.Font.FontSize = 12;
            worksheet.Range("B10:T10").Style.Font.FontSize = 12;
            worksheet.Cell("J7").Style.Font.FontSize = 12;
            worksheet.Cell("O7").Style.Font.FontSize = 12;
            worksheet.Cell("P7").Style.Font.FontSize = 12;
            worksheet.Cell("P9").Style.Font.FontSize = 10;
            worksheet.Cell("Q9").Style.Font.FontSize = 10;
            worksheet.Range("J8:M8").Style.Font.FontSize = 12;
            worksheet.Range("P8:T8").Style.Font.FontSize = 12;
            worksheet.Range("M9:N9").Style.Font.FontSize = 12;
            worksheet.Range("S9:T9").Style.Font.FontSize = 12;
            worksheet.Cell("E7").Style.Font.FontSize = 10;
            worksheet.Cell("C7").Style.Font.FontSize = 10;
            worksheet.Cell("P9").Style.Font.FontSize = 10;
            worksheet.Cell("Q9").Style.Font.FontSize = 10;


            //------------------------------------

            worksheet.Cell("C" + 3).Style.Font.FontSize = 14;
            worksheet.Cell("C" + 2).Style.Font.FontSize = 14;

            worksheet.Cell("G" + 2).Style.Font.FontSize = 14;
            worksheet.Cell("G" + 2).Style.Font.Bold = true;

            //создадим заголовки у столбцов
            worksheet.Cell("A" + 7).Value = "Номер резервуара";
            worksheet.Cell("B" + 7).Value = "Общий уровень нефти";
            worksheet.Cell("B" + 10).Value = "мм";
            worksheet.Cell("C" + 7).Value = "Уровень подтоварной воды и донных отложений";
            worksheet.Cell("C" + 10).Value = "мм";
            worksheet.Cell("D" + 7).Value = "Общий объем";
            worksheet.Cell("D" + 10).Value = "м3";
            worksheet.Cell("E" + 7).Value = "Объем подтоварной воды и донных отложений";
            worksheet.Cell("E" + 10).Value = "м3";
            worksheet.Cell("F" + 7).Value = "Объем нефти";
            worksheet.Cell("F" + 10).Value = "м3";
            worksheet.Cell("G" + 7).Value = "Средняя темп.нефти";
            worksheet.Cell("G" + 10).Value = "С";
            worksheet.Cell("H" + 7).Value = "Плотность нефти при ср.темп.";
            worksheet.Cell("H" + 10).Value = "кг/м3";
            worksheet.Cell("I" + 7).Value = "Масса брутто нефти";
            worksheet.Cell("I" + 10).Value = "т";
            worksheet.Cell("J" + 8).Value = "Вода";
            worksheet.Cell("J" + 10).Value = "%";
            worksheet.Cell("K" + 8).Value = "Соль";
            worksheet.Cell("K" + 10).Value = "%";
            worksheet.Cell("L" + 8).Value = "Мех.прим.";
            worksheet.Cell("L" + 10).Value = "%";
            worksheet.Cell("M" + 10).Value = "%";
            worksheet.Cell("N" + 10).Value = "т";
            worksheet.Cell("O" + 7).Value = "Масса нефти нетто";
            worksheet.Cell("O" + 10).Value = "т";
            worksheet.Cell("P" + 9).Value = "Ниж. норм. ур.";
            worksheet.Cell("P" + 10).Value = "мм"; ;
            worksheet.Cell("Q" + 9).Value = "Объем ниж. норм. ур.";
            worksheet.Cell("Q" + 10).Value = "м3";
            worksheet.Cell("R" + 9).Value = "Масса брутто";
            worksheet.Cell("R" + 10).Value = "т";
            worksheet.Cell("S" + 9).Value = "Масса нетто";
            worksheet.Cell("S" + 10).Value = "т";
            worksheet.Cell("T" + 8).Value = "Товарный";
            worksheet.Cell("T" + 8).Value = "Масса нетто";
            worksheet.Cell("T" + 10).Value = "т";

            worksheet.Cell("J" + 7).Value = "Содержание балласта";
            worksheet.Cell("P" + 7).Value = "В том числе остатки в резервуарах";
            worksheet.Cell("M" + 8).Value = "Всего";
            worksheet.Cell("P" + 8).Value = "Технологический";

            //worksheet.Cell("E7").Style.Font.FontSize = 8;
            //worksheet.Cell("P9").Style.Font.FontSize = 8;
            //worksheet.Cell("Q9").Style.Font.FontSize = 8;

            worksheet.Range("A7:A10").Column(1).Merge();
            worksheet.Range("B7:B9").Column(1).Merge();
            worksheet.Range("C7:C9").Column(1).Merge();
            worksheet.Range("D7:D9").Column(1).Merge();
            worksheet.Range("E7:E9").Column(1).Merge();
            worksheet.Range("F7:F9").Column(1).Merge();
            worksheet.Range("G7:G9").Column(1).Merge();
            worksheet.Range("H7:H9").Column(1).Merge();
            worksheet.Range("N7:N9").Column(1).Merge();
            worksheet.Range("I7:I9").Column(1).Merge();
            worksheet.Range("J8:J9").Column(1).Merge();
            worksheet.Range("K8:K9").Column(1).Merge();
            worksheet.Range("L8:L9").Column(1).Merge();
            worksheet.Range("O7:O9").Column(1).Merge();
            worksheet.Range("J7:N7").Row(1).Merge();
            worksheet.Range("M8:N8").Row(1).Merge();
            worksheet.Range("P7:T7").Row(1).Merge();
            worksheet.Range("P8:S8").Row(1).Merge();

            double VNeftItog = 0;
            double PSred = 0;
            double MassaBalItog = 0;
            double BalProcSred = 0;
            double BalTonnItog = 0;
            double MassaNettoItog = 0;
            double MassaBalMinItog = 0;
            double MassaNettoMinItog = 0;
            double MassaBruttoOstat = 0;
            double VH2OItog = 0;


            for (int ii = 0; ii < Vod.Count; ii++)

            {
                VNeftItog = VNeftItog + Convert.ToDouble(Vod[ii].VNeft);
                PSred = Convert.ToDouble(Vod[ii].P) + PSred;
                MassaBalItog = MassaBalItog + Convert.ToDouble(Vod[ii].MassaBrutto);
                BalProcSred = BalProcSred + Convert.ToDouble(Vod[ii].BalProc);
                BalTonnItog = BalTonnItog + Convert.ToDouble(Vod[ii].BalTonn);
                MassaNettoItog = MassaNettoItog + Convert.ToDouble(Vod[ii].MassaNetto);
                MassaBalMinItog = MassaBalMinItog + Convert.ToDouble(Vod[ii].MBalMin);
                MassaNettoMinItog = MassaNettoMinItog + Convert.ToDouble(Vod[ii].MNettoMin);
                MassaBruttoOstat = MassaBruttoOstat + Convert.ToDouble(Vod[ii].MBalMin);
                VH2OItog = VH2OItog + Convert.ToDouble(Vod[ii].VH2O);

            }

            int uu = -1;

            var groupss = from p in Vod
                          group p by p.type;
            foreach (var gg in groupss)
            {
                int kol = 0;
                double VNeftItog1 = 0;
                double PSred1 = 0;
                double MassaBalItog1 = 0;
                double BalProcSred1 = 0;
                double BalTonnItog1 = 0;
                double MassaNettoItog1 = 0;
                double MassaBalMinItog1 = 0;
                double MassaNettoMinItog1 = 0;
                double MassaBruttoOstat1 = 0;
                double VH2OItog1 = 0;

                foreach (var pp in gg)
                {
                    uu++;
                    worksheet.Cell("A" + (uu + 11)).Value = pp.Rezer;
                    worksheet.Cell("B" + (uu + 11)).Value = pp.Urov;
                    worksheet.Cell("C" + (uu + 11)).Value = pp.UrovH2O;
                    worksheet.Cell("D" + (uu + 11)).Value = Math.Round(pp.V, 3);
                    worksheet.Cell("E" + (uu + 11)).Value = Math.Round(pp.VH2O, 3);
                    worksheet.Cell("F" + (uu + 11)).Value = Math.Round(Convert.ToDouble(pp.VNeft), 1);
                    worksheet.Cell("G" + (uu + 11)).Value = Math.Round(Convert.ToDouble(pp.Temp), 1);
                    worksheet.Cell("H" + (uu + 11)).Value = Math.Round(Convert.ToDouble(pp.P), 1);
                    worksheet.Cell("I" + (uu + 11)).Value = Math.Round(Convert.ToDouble(pp.MassaBrutto), 1);
                    worksheet.Cell("J" + (uu + 11)).Value = Math.Round(Convert.ToDouble(pp.H2O), 2);
                    worksheet.Cell("K" + (uu + 11)).Value = Math.Round(Convert.ToDouble(pp.Salt), 4);
                    worksheet.Cell("L" + (uu + 11)).Value = Math.Round(Convert.ToDouble(pp.Meh), 4);
                    worksheet.Cell("M" + (uu + 11)).Value = Math.Round(Convert.ToDouble(pp.BalProc), 4);
                    worksheet.Cell("N" + (uu + 11)).Value = Math.Round(Convert.ToDouble(pp.BalTonn), 1);
                    worksheet.Cell("O" + (uu + 11)).Value = Math.Round(Convert.ToDouble(pp.MassaNetto), 1);
                    worksheet.Cell("P" + (uu + 11)).Value = pp.HMim;
                    worksheet.Cell("Q" + (uu + 11)).Value = Math.Round(Convert.ToDouble(pp.VMin), 1);
                    worksheet.Cell("R" + (uu + 11)).Value = Math.Round(Convert.ToDouble(pp.MBalMin), 1);
                    worksheet.Cell("S" + (uu + 11)).Value = Math.Round(Convert.ToDouble(pp.MNettoMin), 1);
                    worksheet.Cell("T" + (uu + 11)).Value = null;


                    //-------------Формат ячеек, чтобы после запятой выводилось нужное количество символов (в рот оно ебись)-------------------------------
                    worksheet.Cell("D" + (uu + 11)).Style.NumberFormat.Format = "##0.000";
                    worksheet.Cell("E" + (uu + 11)).Style.NumberFormat.Format = "##0.000";
                    worksheet.Cell("F" + (uu + 11)).Style.NumberFormat.Format = "##0.0";
                    worksheet.Cell("G" + (uu + 11)).Style.NumberFormat.Format = "##0.0";
                    worksheet.Cell("H" + (uu + 11)).Style.NumberFormat.Format = "##0.0";
                    worksheet.Cell("I" + (uu + 11)).Style.NumberFormat.Format = "##0.0";
                    worksheet.Cell("J" + (uu + 11)).Style.NumberFormat.Format = "##0.00";
                    worksheet.Cell("K" + (uu + 11)).Style.NumberFormat.Format = "##0.0000";
                    worksheet.Cell("L" + (uu + 11)).Style.NumberFormat.Format = "##0.0000";
                    worksheet.Cell("M" + (uu + 11)).Style.NumberFormat.Format = "##0.0000";
                    worksheet.Cell("N" + (uu + 11)).Style.NumberFormat.Format = "##0.0";
                    worksheet.Cell("O" + (uu + 11)).Style.NumberFormat.Format = "##0.0";
                    worksheet.Cell("Q" + (uu + 11)).Style.NumberFormat.Format = "##0.0";
                    worksheet.Cell("R" + (uu + 11)).Style.NumberFormat.Format = "##0.0";
                    worksheet.Cell("S" + (uu + 11)).Style.NumberFormat.Format = "##0.0";

                    //-Делаю размер шрифта 14 данных в таблице--------------------------------------------------------

                    worksheet.Range(("A" + (uu + 11)) + ":" + ("T" + (uu + 11))).Style.Font.FontSize = 14;

                    //------------------------------------------------------------------------------------------------

                    //---Задаю нужную ширину ячеек в таблице-------------------------------------------------------------
                    worksheet.Column("A").Width = 12;
                    worksheet.Column("B").Width = 12;
                    worksheet.Column("C").Width = 12;
                    worksheet.Column("D").Width = 12;
                    worksheet.Column("E").Width = 12;
                    worksheet.Column("F").Width = 12;
                    worksheet.Column("G").Width = 12;
                    worksheet.Column("H").Width = 12;
                    worksheet.Column("I").Width = 12;
                    worksheet.Column("J").Width = 12;
                    worksheet.Column("K").Width = 12;
                    worksheet.Column("L").Width = 12;
                    worksheet.Column("M").Width = 12;
                    worksheet.Column("N").Width = 12;
                    worksheet.Column("O").Width = 12;
                    worksheet.Column("P").Width = 12;
                    worksheet.Column("Q").Width = 12;
                    worksheet.Column("R").Width = 12;
                    worksheet.Column("S").Width = 12;
                    worksheet.Column("T").Width = 12;
                    //--------------------------------

                    kol++;
                    VNeftItog1 = VNeftItog1 + Convert.ToDouble(pp.VNeft);
                    PSred1 = Convert.ToDouble(pp.P) + PSred;
                    MassaBalItog1 = MassaBalItog1 + Convert.ToDouble(pp.MassaBrutto);
                    BalProcSred1 = BalProcSred1 + Convert.ToDouble(pp.BalProc);
                    BalTonnItog1 = BalTonnItog1 + Convert.ToDouble(pp.BalTonn);
                    MassaNettoItog1 = MassaNettoItog1 + Convert.ToDouble(pp.MassaNetto);
                    MassaBalMinItog1 = MassaBalMinItog1 + Convert.ToDouble(pp.MBalMin);
                    MassaNettoMinItog1 = MassaNettoMinItog1 + Convert.ToDouble(pp.MNettoMin);
                    MassaBruttoOstat1 = MassaBruttoOstat1 + Convert.ToDouble(pp.MBalMin);
                    VH2OItog1 = VH2OItog1 + Convert.ToDouble(pp.VH2O);

                }
                uu++;
                worksheet.Cell("A" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.TeaGreen;
                worksheet.Cell("B" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.TeaGreen;
                worksheet.Cell("C" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.TeaGreen;
                worksheet.Cell("D" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.TeaGreen;
                worksheet.Cell("E" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.TeaGreen;
                worksheet.Cell("F" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.TeaGreen;
                worksheet.Cell("G" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.TeaGreen;
                worksheet.Cell("H" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.TeaGreen;
                worksheet.Cell("I" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.TeaGreen;
                worksheet.Cell("J" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.TeaGreen;
                worksheet.Cell("K" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.TeaGreen;
                worksheet.Cell("L" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.TeaGreen;
                worksheet.Cell("M" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.TeaGreen;
                worksheet.Cell("N" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.TeaGreen;
                worksheet.Cell("O" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.TeaGreen;
                worksheet.Cell("P" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.TeaGreen;
                worksheet.Cell("Q" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.TeaGreen;
                worksheet.Cell("R" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.TeaGreen;
                worksheet.Cell("S" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.TeaGreen;
                worksheet.Cell("T" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.TeaGreen;

                worksheet.Range(("A" + (uu + 11)) + ":" + ("T" + (uu + 11))).Style.Font.FontSize = 14;

                //------Делаем жирным шрифтом итоги-------------------------

                worksheet.Cell("F" + (uu + 11)).Style.Font.Bold = true;
                worksheet.Cell("E" + (uu + 11)).Style.Font.Bold = true;
                worksheet.Cell("H" + (uu + 11)).Style.Font.Bold = true;
                worksheet.Cell("I" + (uu + 11)).Style.Font.Bold = true;
                worksheet.Cell("N" + (uu + 11)).Style.Font.Bold = true;
                worksheet.Cell("O" + (uu + 11)).Style.Font.Bold = true;
                worksheet.Cell("S" + (uu + 11)).Style.Font.Bold = true;
                worksheet.Cell("R" + (uu + 11)).Style.Font.Bold = true;
                worksheet.Cell("M" + (uu + 11)).Style.Font.Bold = true;


                worksheet.Cell("A" + (uu + 11)).Value = "ИТОГО ПО ГРУППЕ: ";
                worksheet.Cell("F" + (uu + 11)).Value = VNeftItog1;
                worksheet.Cell("E" + (uu + 11)).Value = VH2OItog1;
                worksheet.Cell("I" + (uu + 11)).Value = MassaBalItog1;
                worksheet.Cell("H" + (uu + 11)).Value = Math.Round(MassaBalItog1 / VNeftItog1 * 1000, 1);
                worksheet.Cell("N" + (uu + 11)).Value = BalTonnItog1;
                worksheet.Cell("O" + (uu + 11)).Value = MassaNettoItog1;
                worksheet.Cell("S" + (uu + 11)).Value = MassaNettoMinItog1;
                worksheet.Cell("R" + (uu + 11)).Value = MassaBruttoOstat1;
                // worksheet.Cell("M" + (uu + 11)).Value = Math.Round((MassaBalItog1 - MassaNettoItog1) * 100 / MassaBalItog1, 3);

                //--Стили ИТОГИ ПО ГРУППЕ делаю нужное количество знаков после запятой---------------------------------------
                worksheet.Cell("F" + (uu + 11)).Style.NumberFormat.Format = "##0.0";
                worksheet.Cell("E" + (uu + 11)).Style.NumberFormat.Format = "##0.0";
                worksheet.Cell("H" + (uu + 11)).Style.NumberFormat.Format = "##0.0";
                worksheet.Cell("I" + (uu + 11)).Style.NumberFormat.Format = "##0.0";
                worksheet.Cell("N" + (uu + 11)).Style.NumberFormat.Format = "##0.0";
                worksheet.Cell("O" + (uu + 11)).Style.NumberFormat.Format = "##0.0";
                worksheet.Cell("R" + (uu + 11)).Style.NumberFormat.Format = "##0.0";
                worksheet.Cell("S" + (uu + 11)).Style.NumberFormat.Format = "##0.0";
                //-----------------------------------------------------------------------------------------------------------
            }
            uu++;
            worksheet.Cell("A" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.BabyBlue;
            worksheet.Cell("B" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.BabyBlue;
            worksheet.Cell("C" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.BabyBlue;
            worksheet.Cell("D" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.BabyBlue;
            worksheet.Cell("E" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.BabyBlue;
            worksheet.Cell("F" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.BabyBlue;
            worksheet.Cell("G" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.BabyBlue;
            worksheet.Cell("H" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.BabyBlue;
            worksheet.Cell("I" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.BabyBlue;
            worksheet.Cell("J" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.BabyBlue;
            worksheet.Cell("K" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.BabyBlue;
            worksheet.Cell("L" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.BabyBlue;
            worksheet.Cell("M" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.BabyBlue;
            worksheet.Cell("N" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.BabyBlue;
            worksheet.Cell("O" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.BabyBlue;
            worksheet.Cell("P" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.BabyBlue;
            worksheet.Cell("Q" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.BabyBlue;
            worksheet.Cell("R" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.BabyBlue;
            worksheet.Cell("S" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.BabyBlue;
            worksheet.Cell("T" + (uu + 11)).Style.Fill.BackgroundColor = XLColor.BabyBlue;

            worksheet.Range(("A" + (uu + 11)) + ":" + ("T" + (uu + 11))).Style.Font.FontSize = 14;

            //--------Делаю жирным итоги внизу-------------------------

            worksheet.Cell("F" + (uu + 11)).Style.Font.Bold = true;
            worksheet.Cell("E" + (uu + 11)).Style.Font.Bold = true;
            worksheet.Cell("H" + (uu + 11)).Style.Font.Bold = true;
            worksheet.Cell("I" + (uu + 11)).Style.Font.Bold = true;
            worksheet.Cell("N" + (uu + 11)).Style.Font.Bold = true;
            worksheet.Cell("O" + (uu + 11)).Style.Font.Bold = true;
            worksheet.Cell("S" + (uu + 11)).Style.Font.Bold = true;
            worksheet.Cell("T" + (uu + 11)).Style.Font.Bold = true;
            worksheet.Cell("R" + (uu + 11)).Style.Font.Bold = true;
            worksheet.Cell("M" + (uu + 11)).Style.Font.Bold = true;

            worksheet.Cell("A" + (uu + 11)).Value = "ИТОГО:";
            worksheet.Cell("E" + (uu + 11)).Value = Math.Round(VH2OItog, 0);
            worksheet.Cell("F" + (uu + 11)).Value = Math.Round(VNeftItog, 0);
            //worksheet.Cell("H" + (uu + 11)).Value = Math.Round(PSred / Vod.Count, 1);
            worksheet.Cell("H" + (uu + 11)).Value = Math.Round(MassaBalItog / VNeftItog * 1000, 1);
            worksheet.Cell("I" + (uu + 11)).Value = Math.Round(MassaBalItog, 0);
            worksheet.Cell("N" + (uu + 11)).Value = Math.Round(BalTonnItog, 0);
            worksheet.Cell("O" + (uu + 11)).Value = Math.Round(MassaNettoItog, 0);
            worksheet.Cell("S" + (uu + 11)).Value = Math.Round(MassaNettoMinItog, 0);
            //    worksheet.Cell("T" + (uu + 11)).Value = Math.Round((MassaNettoItog - MassaNettoMinItog),0);
            worksheet.Cell("T" + (uu + 11)).Value = Math.Round(MassaNettoItog, 0) - Math.Round(MassaNettoMinItog, 0);
            worksheet.Cell("R" + (uu + 11)).Value = Math.Round(MassaBruttoOstat, 0);
            worksheet.Cell("M" + (uu + 11)).Value = Math.Round((MassaBalItog - MassaNettoItog) * 100 / MassaBalItog, 4);


            var rngTable = worksheet.Range("A7:T" + (uu + 11));
            rngTable.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            rngTable.Style.Border.BottomBorder = XLBorderStyleValues.Thin;


            //---Здесь описаны стили для шапки таблиц-----------------------------------//
            var rngTable111 = worksheet.Range("A7:T" + 10);
            rngTable111.Style.Border.RightBorder = XLBorderStyleValues.Medium;
            rngTable111.Style.Border.BottomBorder = XLBorderStyleValues.Medium;
            rngTable111.Style.Border.LeftBorder = XLBorderStyleValues.Medium;
            rngTable111.Style.Border.TopBorder = XLBorderStyleValues.Medium;
            rngTable111.Style.Fill.BackgroundColor = XLColor.TeaGreen;
            rngTable111.Style.Alignment.WrapText = true;
            rngTable111.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


            //-----------Вывод ресурсов нефти из БД.-------------------------------------------------------------------------

            foreach (var ii in Resursy)
            {
                //--Выводим ресурсы хранения нефти шрифтом=12----------------------------------
                worksheet.Cell("C" + (uu + 13)).Style.Font.FontSize = 12;
                worksheet.Cell("O" + (uu + 13)).Style.Font.FontSize = 12;
                //--------------------------------------
                worksheet.Cell("O" + (uu + 13)).Style.NumberFormat.Format = "##0.0";
                worksheet.Cell("C" + (uu + 13)).Value = ii.Naimenovanie.Trim();
                worksheet.Cell("O" + (uu + 13)).Value = ii.VNefti;
                worksheet.Cell("P" + (uu + 13)).Value = "тонн";
                uu++;

            }


            string chlenyBottom = "Председатель комиссии________________" + predsFIO + "    Члены комиссии: ";
            foreach (var iii in Podpis)
            {
                chlenyBottom = chlenyBottom + "          ________________" + iii.FIO.Trim() + ", ";
            }

            worksheet.Cell("A" + (uu + 14)).Style.Font.FontSize = 12;

            worksheet.Range("A" + (uu + 14) + ":" + "T" + (uu + 13)).Row(1).Merge();
            worksheet.Range("A" + (uu + 14) + ":" + "T" + (uu + 13)).Style.Alignment.WrapText = true;
            worksheet.Cell("A" + (uu + 14)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
            worksheet.Cell("A" + (uu + 14)).Value = chlenyBottom;
            worksheet.Row(uu + 14).Height = 30;

            //--------------------------------------------------------------------------------------

            //---------------------Вывод МОЛ--------------------------------------------------------
            worksheet.Cell("A" + (uu + 15)).Style.Font.FontSize = 12;

            worksheet.Cell("A" + (uu + 15)).Value = MOL;
            worksheet.Row(uu + 15).Height = 38;
            //--------------------------------------------------------------------------------------


            // вернем пользователю файл без сохранения его на сервере

            using (MemoryStream stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Report.xlsx");
            }
        }

        //------------------------------------------------------------------------------------------------------

        // Редактирование члена комиссии//

        public ActionResult InventEdit(int ID)
        {
            TankInv TanI = new TankInv();
            TanI = db.TankInv.FirstOrDefault(a => a.Id == ID);
            return PartialView(TanI);
        }
        //-------------------------------//

        //Сохранение редактирования члена комиссии------------//
        [HttpPost]
        public ActionResult InventEditSave(int ID, string InvRezer, string InvUrov, string InvUrovH2O, string InvV, string InvVH2O, string InvVNeft, string InvTemp, string InvP, string InvMassaBrutto, string InvH2O, string InvSalt, string InvMeh, string InvBalProc, string InvBalTonn, string InvMassaNetto, string InvHMim, string InvVMin, string InvMNettoMin, string InvVTeh)
        {
            try
            {
                TankInv TanKINVE = new TankInv();
                TanKINVE = db.TankInv.FirstOrDefault(s => s.Id == ID);

                TanKINVE.Rezer = Convert.ToInt32(InvRezer);
                TanKINVE.Urov = Convert.ToDecimal(InvUrov);
                TanKINVE.UrovH2O = Convert.ToDecimal(InvUrovH2O);
                TanKINVE.V = Convert.ToDecimal(InvV);
                TanKINVE.VH2O = Convert.ToDecimal(InvVH2O);
                TanKINVE.VNeft = Convert.ToDecimal(InvVNeft);
                TanKINVE.Temp = Convert.ToDecimal(InvTemp);
                TanKINVE.P = Convert.ToDecimal(InvP);
                TanKINVE.MassaBrutto = Convert.ToDecimal(InvMassaBrutto);
                TanKINVE.H2O = Convert.ToDecimal(InvH2O);
                TanKINVE.Salt = Convert.ToDecimal(InvSalt);
                TanKINVE.Meh = Convert.ToDecimal(InvMeh);
                TanKINVE.BalProc = Convert.ToDecimal(InvBalProc);
                TanKINVE.BalTonn = Convert.ToDecimal(InvBalTonn);
                TanKINVE.MassaNetto = Convert.ToDecimal(InvMassaNetto);
                TanKINVE.HMim = Convert.ToDecimal(InvHMim);
                TanKINVE.VMin = Convert.ToDecimal(InvVMin);
                TanKINVE.MNettoMin = Convert.ToDecimal(InvMNettoMin);
                TanKINVE.VTeh = Convert.ToDecimal(InvVTeh);

                db.SaveChanges();

                ViewBag.Message = "Инвентаризация успешно изменена";

            }
            catch (Exception e)
            {
                ViewBag.Message = "Ошибка. Текст ошибки: " + e.ToString();
            }

            return PartialView();
        }
        //-----------------------------//
        public string GetUrovH2O(int ID, int IDFilial, int InvRezerEdit, string InvUrovEdit, string InvUrovH2OEdit, string InvVEdit, string InvVH2OEdit, string InvVNeftEdit, string InvTempEdit, string InvPEdit,
        string InvMassaBruttoEdit, string InvH2OEdit, string InvSaltEdit, string InvMehEdit, string InvBalProcEdit, string InvBalTonnEdit, string InvMassaNettoEdit, string InvHMimEdit,
        string InvVMinEdit, string InvMNettoMinEdit, string InvMBalMinEdit)
        {
            //---------------------------------------------------------------------------- 
            double VEdit = Convert.ToDouble(InvVEdit);
            double InvUrov = Convert.ToDouble(InvUrovEdit);
            double InvUrovH2O = Convert.ToDouble(InvUrovH2OEdit);
            double InvUrovNeft = InvUrov - InvUrovH2O;
            double VH2OEdit = Convert.ToDouble(InvVH2OEdit);
            double VNeftEdit = Convert.ToDouble(InvVNeftEdit);
            double MassaBruttoEdit = Convert.ToDouble(InvMassaBruttoEdit);
            double BalProcEdit = Convert.ToDouble(InvBalProcEdit);
            double BalTonnEdit = Convert.ToDouble(InvBalTonnEdit);
            double MassaNettoEdit = Convert.ToDouble(InvMassaNettoEdit);
            double UminEdit;
            double UmaxEdit;
            double UpercentEdit;
            double VminEdit;
            double VmaxEdit;

            double UminEditH2O;
            double UmaxEditH2O;
            double UpercentEditH2O;
            double VminEditH2O;
            double VmaxEditH2O;
            double VNeft;
            double MassaBrutto;
            double BalProc;
            double BalTonn;
            double MassaNetto;
            double HMin;
            double VMin;

            int type;
            double VRasch;
            double VH2ORasch;
            double Kst;
            
            
                //Проверяем тип резервуара (ЖБР или РВС)
                if (InvRezerEdit == 1 | InvRezerEdit == 2 | InvRezerEdit == 3 | InvRezerEdit == 4 | InvRezerEdit == 5 | InvRezerEdit == 6 | InvRezerEdit == 7 | InvRezerEdit == 8 | InvRezerEdit == 9 | InvRezerEdit == 10)
                {
                    Kst = 0.00001;
                }
                else
                {
                    Kst = 0.0000125;
                }


                List<calibration> calibrationListEdit = new List<calibration>();
                calibrationListEdit = db.calibration.Where(f => f.filialid == IDFilial & f.tankid == InvRezerEdit).OrderBy(h => h.oillevel).ToList();

                //Рассчет объема общего
                if (calibrationListEdit == null)
                {
                    VEdit = 0;
                    VRasch = 0;
                }
                else
                {
                    if (calibrationListEdit.LastOrDefault(g => g.oillevel <= Convert.ToDecimal(InvUrovEdit)) == null)
                    {
                        UminEdit = 0;
                    }
                    else
                    {
                        UminEdit = calibrationListEdit.LastOrDefault(g => g.oillevel <= Convert.ToDouble(InvUrovEdit)).oillevel;
                    }
                    UmaxEdit = calibrationListEdit.FirstOrDefault(j => j.oillevel > Convert.ToDouble(InvUrovEdit)).oillevel;

                    if (calibrationListEdit.LastOrDefault(g => g.oillevel <= Convert.ToDouble(InvUrovEdit)) == null)
                    {
                        VminEdit = 0;
                        
                    }
                    else
                    {
                        VminEdit = calibrationListEdit.LastOrDefault(g => g.oillevel <= Convert.ToDouble(InvUrovEdit)).oilvolume;
                    }
                    VmaxEdit = calibrationListEdit.FirstOrDefault(j => j.oillevel > Convert.ToDouble(InvUrovEdit)).oilvolume;
                    UpercentEdit = (Convert.ToDouble(InvUrovEdit) - UminEdit) / (UmaxEdit - UminEdit);
                    VEdit = VminEdit + (VmaxEdit - VminEdit) * UpercentEdit;

                  TankInv TanEdit1 = new TankInv();
                 
                //Рассчитываем общий объем (нефть и подтоварная вода) по формуле
                 VRasch = VEdit * (1 + 2 * Kst * (Convert.ToDouble(InvTempEdit) - 20));

            }

                //--------Рассчет объема подтоварной воды----------------------------------------------------------------
                if (calibrationListEdit == null & InvUrovH2O == 0)
                {
                    VH2OEdit = 0;
                    VH2ORasch = 0;
                }
                else
                {
                    if (calibrationListEdit.LastOrDefault(g => g.oillevel <= Convert.ToDouble(InvUrovH2OEdit)) == null)
                    {
                        UminEditH2O = 0;
                    }
                    else
                    {
                        UminEditH2O = calibrationListEdit.LastOrDefault(g => g.oillevel <= Convert.ToDouble(InvUrovH2OEdit)).oillevel;
                    }
                    UmaxEditH2O = calibrationListEdit.FirstOrDefault(j => j.oillevel > Convert.ToDouble(InvUrovH2OEdit)).oillevel;

                    if (calibrationListEdit.LastOrDefault(g => g.oillevel <= Convert.ToDouble(InvUrovH2OEdit)) == null)
                    {
                        VminEditH2O = 0;
                    }
                    else
                    {
                        VminEditH2O = calibrationListEdit.LastOrDefault(g => g.oillevel <= Convert.ToDouble(InvUrovH2OEdit)).oilvolume;
                    }
                    VmaxEditH2O = calibrationListEdit.FirstOrDefault(j => j.oillevel > Convert.ToDouble(InvUrovH2OEdit)).oilvolume;
                    UpercentEditH2O = (Convert.ToDouble(InvUrovH2OEdit) - UminEditH2O) / (UmaxEditH2O - UminEditH2O);
                    if (InvUrovH2O == 0)
                    {
                        VH2OEdit = 0;
                        VH2ORasch = 0;
                    }
                    else
                    {
                        VH2OEdit = VminEditH2O + (VmaxEditH2O - VminEditH2O) * UpercentEditH2O;
                    //Рассчитываем объем подтоварной воды по формуле
                    VH2ORasch = VH2OEdit * (1 + 2 * Kst * (Convert.ToDouble(InvTempEdit) - 20));
                }
                }
                //-------------------------------------------------------------------------------
                VNeft = VRasch - VH2ORasch; // Объем нефти;
                MassaBrutto = VNeft * Convert.ToDouble(InvPEdit) / 1000; // Масса нефти брутто;
                BalProc = Convert.ToDouble(InvH2OEdit) + Convert.ToDouble(InvSaltEdit) + Convert.ToDouble(InvMehEdit); //Рассчет балласта в процентах;
                BalTonn = MassaBrutto * BalProc / 100; //Рассчет балласта в тоннах;
                MassaNetto = MassaBrutto - BalTonn;
                HMin = Convert.ToDouble(InvHMimEdit);
                VMin = Convert.ToDouble(InvVMinEdit);

                TankInv TanEdit = new TankInv();
                TanEdit.Id = ID;
                TanEdit.Filial = IDFilial;
                TanEdit.Rezer = InvRezerEdit;
                TanEdit.Urov = Convert.ToDecimal(InvUrov);
                TanEdit.UrovH2O = Convert.ToDecimal(InvUrovH2O);
                TanEdit.V = Math.Round(Convert.ToDecimal(VEdit), 3);
                TanEdit.VH2O = Math.Round(Convert.ToDecimal(VH2OEdit), 3);
                TanEdit.UrovNeft = Convert.ToDecimal(InvUrovNeft);
                TanEdit.VNeft = Math.Round(Convert.ToDecimal(VNeft), 3);
                TanEdit.Temp = Convert.ToDecimal(InvTempEdit);
                TanEdit.P = Convert.ToDecimal(InvPEdit);
                TanEdit.MassaBrutto = Math.Round(Convert.ToDecimal(MassaBrutto), 3);
                TanEdit.H2O = Convert.ToDecimal(InvH2OEdit);
                TanEdit.Salt = Convert.ToDecimal(InvSaltEdit);
                TanEdit.Meh = Convert.ToDecimal(InvMehEdit);
                TanEdit.BalProc = Math.Round(Convert.ToDecimal(BalProc), 3);
                TanEdit.BalTonn = Math.Round(Convert.ToDecimal(BalTonn), 3);
                TanEdit.MassaNetto = Math.Round(Convert.ToDecimal(MassaNetto), 3);
                TanEdit.HMim = Convert.ToDecimal(HMin);
                TanEdit.VMin = Convert.ToDecimal(VMin);
                TanEdit.MBalMin = Convert.ToDecimal(VMin) * Convert.ToDecimal(InvPEdit) / 1000;
                TanEdit.MNettoMin = TanEdit.MBalMin - (TanEdit.MBalMin * Math.Round(Convert.ToDecimal(BalProc), 3) / 100);

                ViewBag.TanEdit = TanEdit;
                //ViewBag.TanEdit = 777;
            
            return JsonConvert.SerializeObject(TanEdit);

        }

        //-------------------------------------------------------------------------
        public async Task<ActionResult> LetGradTable(int filial2, DateTime dataHim, DateTime dataInvent, string timeInvent)
        {
            filials hhh = new filials();
            tankinfo hj = new tankinfo();

            List<TankInv> ListTankInv = new List<TankInv>();

            List<double> TEST = new List<double>();

            //------------------------------------------------------------------------------------------------------------------------------------------------------------

            //список тиров резервуаров
            //--------------------------------------------------
            List<trl_tank> trl = new List<trl_tank>();
            trl = db.trl_tank.Where(p => p.FilialID == filial2).OrderBy(p => p.TypeID).ThenBy(p => p.TankID).ToList();

            List<int> typ = new List<int>();
            foreach (var t in trl)
            {
                {
                    if (t.TypeID != 0)
                        typ.Add(t.TypeID);
                }
            }

            List<int> typDyst = new List<int>();

            typDyst = typ.Distinct().ToList();
            ViewBag.typ = typDyst;

            List<TankInv> ListTank = new List<TankInv>();
            ListTank.Clear();

            string timeNow = DateTime.Now.ToLongTimeString().Substring(3);

            var spis = await Asopn.Spis(filial2, dataHim, dataInvent, timeInvent);

            DateTime dattt = DateTime.Now;
            try
            {
                foreach (var item in spis)

                {
                    ////---Запись таблицы в БД---------------------------------------------------------------------
                    TankInv TankinV = new TankInv();
                    TankinV.Data = Convert.ToDateTime((Convert.ToString(item.Data)).Replace(":00:00", ":" + timeNow));
                    //TankinV.Data = item.Data.AddMinutes(Convert.ToDouble(timeNow));                    
                    TankinV.Filial = item.Filial;
                    TankinV.Rezer = item.Rezer;
                    TankinV.Urov = item.Urov;
                    TankinV.UrovH2O = item.UrovH2O;
                    TankinV.UrovNeft = item.UrovNeft;
                    TankinV.V = Convert.ToDecimal(Math.Round(item.V, 3));
                    TankinV.Temp = Convert.ToDecimal(Math.Round(item.Temp, 2));
                    TankinV.P = Convert.ToDecimal(Math.Round(item.P, 2));
                    TankinV.MassaBrutto = Convert.ToDecimal(Math.Round(item.MassaBrutto, 3));
                    TankinV.H2O = Convert.ToDecimal(Math.Round(item.H2O, 4));
                    TankinV.Salt = Convert.ToDecimal(Math.Round(item.Salt,4));
                    TankinV.Meh = Convert.ToDecimal(Math.Round(item.Meh, 4));
                    TankinV.BalProc = Convert.ToDecimal(Math.Round(item.H2O, 4)) + Convert.ToDecimal(Math.Round(item.Salt, 4)) + Convert.ToDecimal(Math.Round(item.Meh, 4));
                    //TankinV.BalProc = Convert.ToDecimal(item.BalProc);
                    TankinV.BalTonn = Convert.ToDecimal(item.BalTonn);
                    TankinV.MassaNetto = Convert.ToDecimal(Math.Round(item.MassaNetto, 1));
                    TankinV.HMim = Convert.ToDecimal(Math.Round(item.Hmin, 0));
                    TankinV.VMin = Convert.ToDecimal(Math.Round(item.Vmin, 1));
                    TankinV.MBalMin = Convert.ToDecimal(Math.Round(item.dMBalmin, 1));
                    TankinV.MNettoMin = Convert.ToDecimal(Math.Round(item.dMNettomin, 1));
                    TankinV.type = item.type;
                    TankinV.VH2O = Convert.ToDecimal(item.VH2O);
                    TankinV.VNeft = Convert.ToDecimal(item.VNeft);


                    ListTank.Add(TankinV);
                    Asopn.db1.TankInv.Add(TankinV);
                    Asopn.db1.SaveChanges();

                    //--------------------------------------------------
                    dattt = Convert.ToDateTime((Convert.ToString(item.Data)).Replace(":00:00", ":" + timeNow));
                }

                //--Рассчет итогов в формировании акта инвентаризации----------

                List<TankInv> TableInv = new List<TankInv>();
                TableInv = db.TankInv.Where(j => j.Filial == filial2 & j.Data == dattt).OrderBy(h => h.type).ThenBy(h => h.Rezer).ToList();

                int kol = 0;
                double VNeftItog1 = 0;
                double PSred1 = 0;
                double MassaBalItog1 = 0;
                double BalProcSred1 = 0;
                double BalTonnItog1 = 0;
                double MassaNettoItog1 = 0;
                double MassaBalMinItog1 = 0;
                double MassaNettoMinItog1 = 0;
                double MassaBruttoOstat1 = 0;

                foreach (var t in TableInv)
                {
                    kol++;
                    VNeftItog1 = VNeftItog1 + Convert.ToDouble(t.VNeft);
                    PSred1 = Convert.ToDouble(t.P) + PSred1;
                    MassaBalItog1 = MassaBalItog1 + Convert.ToDouble(t.MassaBrutto);
                    BalProcSred1 = BalProcSred1 + Convert.ToDouble(t.BalProc);
                    BalTonnItog1 = BalTonnItog1 + Convert.ToDouble(t.BalTonn);
                    MassaNettoItog1 = MassaNettoItog1 + Convert.ToDouble(t.MassaNetto);
                    MassaBalMinItog1 = MassaBalMinItog1 + Convert.ToDouble(t.MBalMin);
                    MassaNettoMinItog1 = MassaNettoMinItog1 + Convert.ToDouble(t.MNettoMin);
                    MassaBruttoOstat1 = MassaBruttoOstat1 + Convert.ToDouble(t.MNettoMin);
                }
                //---------------------------------------------
                ViewBag.VNeftItog2 = VNeftItog1;
                ViewBag.PSred2 = Math.Round(MassaBalItog1 / VNeftItog1, 1);
                ViewBag.MassaBalItog2 = Math.Round(MassaBalItog1, 1);
                ViewBag.BalProcSred2 = Math.Round(BalProcSred1 / kol, 1);
                ViewBag.BalTonnItog2 = Math.Round(BalTonnItog1, 1);
                ViewBag.MassaNettoItog2 = Math.Round(MassaNettoItog1, 1);
                ViewBag.MassaBalMinItog2 = Math.Round(MassaBalMinItog1, 1);
                ViewBag.MassaNettoMinItog2 = Math.Round(MassaNettoMinItog1, 1);
                ViewBag.MassaBruttoOstat2 = Math.Round(MassaBruttoOstat1, 1);

                //-------------------------------------------------------------

                ViewBag.TableInv = ListTank;
                return PartialView(ListTank);
            }
            catch (Exception ex)
            {                
                return PartialView(ex.Message);
            }

        }
        //----------------------------------------------------------------------------------------------------------------------------

        //----------Добавление резервуара Мозыря-----------------------//

        public ActionResult AddRezerMoz()
        {
            SelectList typeMoz = new SelectList(db.trl_tanktype, "ID", "Name");
            ViewBag.typeMoz = typeMoz;

            return PartialView();
        }
        //--------------------------//
        //-----Сохранение добавления резервуара Мозыря-----------//
        [HttpPost]
        public ActionResult RezerMozSave(string NaimenovanRezerMoz, int typeMoz, double UrovMinMoz, double VminMoz, int tankidMoz)
        {
            try
            {
                trl_tank RezMoz = new trl_tank();
                RezMoz.FilialID = 1;
                RezMoz.Name = NaimenovanRezerMoz;
                RezMoz.TypeID = typeMoz;
                RezMoz.MinDopLevel = UrovMinMoz;
                RezMoz.MinDopVol = VminMoz;
                RezMoz.TankID = tankidMoz;

                db.trl_tank.Add(RezMoz);

                db.SaveChanges();

                ViewBag.Message = "Резервуар успешно добавлен!";
            }
            catch (Exception ex)
            {


                ViewBag.Message = ex.ToString();
            }

            return PartialView();
        }
        //----------------------------------//

        //----------Добавление резервуара Новополоцка-----------------------//

        public ActionResult AddRezerNovop()
        {
            SelectList typeNovop = new SelectList(db.trl_tanktype, "ID", "Name");
            ViewBag.typeNovop = typeNovop;

            return PartialView();
        }
        //--------------------------//
        //-----Сохранение добавления резервуара Новополоцка-----------//
        [HttpPost]
        public ActionResult RezerNovopSave(string NaimenovanRezerNovop, int typeNovop, double UrovMinNovop, double VminNovop, int tankidNovop)
        {
            try
            {
                trl_tank RezNovop = new trl_tank();
                RezNovop.FilialID = 2;
                RezNovop.Name = NaimenovanRezerNovop;
                RezNovop.TypeID = typeNovop;
                RezNovop.MinDopLevel = UrovMinNovop;
                RezNovop.MinDopVol = VminNovop;
                RezNovop.TankID = tankidNovop;

                db.trl_tank.Add(RezNovop);

                db.SaveChanges();

                ViewBag.Message = "Резервуар успешно добавлен!";
            }
            catch (Exception ex)
            {


                ViewBag.Message = ex.ToString();
            }

            return PartialView();
        }
        //----------------------------------//

        //-------------------------------------
        // удаление резервуара//

        public ActionResult DeleteRezer(int ID)
        {
            trl_tank rezer = new trl_tank();
            rezer = db.trl_tank.FirstOrDefault(a => a.ID == ID);
            return PartialView(rezer);
        }
        //-----------------------------//

        // Подтверждение удаления резервуара//
        public ActionResult DeleteRezerOK(int ID)
        {
            try
            {

                trl_tank rezerv = new trl_tank();
                rezerv = db.trl_tank.FirstOrDefault(a => a.ID == ID);
                db.trl_tank.Remove(rezerv);
                db.SaveChanges();

                ViewBag.Message = "Резервуар успешно удален!";
            }
            catch
            {
                ViewBag.Message = "Ошибка удаления";
            }

            return PartialView();
        }
        // Редактирование резервуара//

        public ActionResult RezerEdit(int ID)
        {

            trl_tank rez = new trl_tank();
            rez = db.trl_tank.FirstOrDefault(a => a.ID == ID);

            SelectList rez1 = new SelectList(db.trl_tanktype, "ID", "Name");
            ViewBag.rez1 = rez1;

            return PartialView(rez);

        }
        //-------------------------------//

        //Сохранение редактирования резервуара------------//
        [HttpPost]
        public ActionResult RezerEditSave(int ID, int NumberRezEdit, string NameRezEdit, int typeRezEdit, double LevelRezEdit, double VRezEdit)
        {
            try
            {
                trl_tank rezEdit = new trl_tank();
                rezEdit = db.trl_tank.FirstOrDefault(s => s.ID == ID);

                rezEdit.TankID = NumberRezEdit;
                rezEdit.Name = NameRezEdit.Trim();
                rezEdit.TypeID = typeRezEdit;
                rezEdit.MinDopLevel = LevelRezEdit;
                rezEdit.MinDopVol = VRezEdit;


                db.SaveChanges();

                ViewBag.Message = "Резервуар успешно изменен";

            }
            catch (Exception e)
            {
                ViewBag.Message = "Ошибка. Текст ошибки: " + e.ToString();
            }

            return PartialView();
        }
        //-----------------------------//

        //--------Корректировка значений по Мозырю и Новополоцку----------------------------------------------------------

        public ActionResult CorrectMoz()
        {
            List<Correct> CorMoz = new List<Correct>();
            CorMoz = db.Correct.Where(h => h.filial == 1).ToList();

            return View(CorMoz);
        }

        public ActionResult CorrectNovop()
        {
            List<Correct> corNovop = new List<Correct>();
            corNovop = db.Correct.Where(h => h.filial == 2).ToList();

            return View(corNovop);
        }


        //----------Добавление корректировки ЛПДС Мозырь-----------------------//

        public ActionResult AddCorrectMoz()
        {
            SelectList corMoz = new SelectList(db.trl_tank.Where(g=>g.FilialID == 1), "id", "name");
            ViewBag.corMoz = corMoz;

            return PartialView();
        }
        //--------------------------//
        //-----Сохранение добавления корректировки ЛПДС Мозырь----------//
        [HttpPost]
        public ActionResult CorrectMozSave(int tankidMoz, decimal? UrovCMoz, decimal? TempCMoz)
        {
            try
            {
                Correct corM = new Correct();
                corM.filial = 1;
                corM.tankid = tankidMoz;
                corM.Uroven = UrovCMoz;
                corM.Temp = TempCMoz;

                db.Correct.Add(corM);

                db.SaveChanges();

                ViewBag.Message = "Запись успешно добавлена!";
            }
            catch (Exception ex)
            {


                ViewBag.Message = ex.ToString();
            }

            return PartialView();
        }
        //----------------------------------//

        //----------Добавление корректировки ЛПДС Полоцк-----------------------//

        public ActionResult AddCorrectNovop()
        {
            SelectList corNovop = new SelectList(db.trl_tank.Where(g=>g.FilialID == 2), "TankID", "name");
            ViewBag.corNovop = corNovop;

            return PartialView();
        }
        //--------------------------//
        //-----Сохранение корректировки ЛПДС Полоцк-----------//
        [HttpPost]
        public ActionResult CorrectNovopSave(int tankidNovop, decimal? UrovCNovop, decimal? TempCNovop)
        {
            try
            {
                Correct corN = new Correct();
                corN.filial = 2;
                corN.tankid = tankidNovop;
                corN.Uroven = UrovCNovop;
                corN.Temp = TempCNovop;

                db.Correct.Add(corN);

                db.SaveChanges();

                ViewBag.Message = "Запись успешно добавлена!";
            }
            catch (Exception ex)
            {


                ViewBag.Message = ex.ToString();
            }

            return PartialView();
        }
        //----------------------------------//
        //-------------------------------------
        // удаление корректировки//

        public ActionResult DeleteCorrect(int ID)
        {
            Correct cor = new Correct();
            cor = db.Correct.FirstOrDefault(a => a.id == ID);
            return PartialView(cor);
        }
        //-----------------------------//

        // Подтверждение удаления корректировки//
        public ActionResult DeleteCorrectOK(int ID)
        {
            try
            {

                Correct corr = new Correct();
                corr = db.Correct.FirstOrDefault(a => a.id == ID);
                db.Correct.Remove(corr);
                db.SaveChanges();

                ViewBag.Message = "Корректировка удалена!";
            }
            catch
            {
                ViewBag.Message = "Ошибка удаления";
            }

            return PartialView();
        }
        // Редактирование корректировки//

        public ActionResult CorrectEdit(int ID)
        {

            Correct cor = new Correct();
            cor = db.Correct.FirstOrDefault(a => a.id == ID);

            SelectList cor1 = new SelectList(db.trl_tank, "id", "name");
            ViewBag.cor1 = cor1;

            return PartialView(cor);

        }
        //-------------------------------//

        //Сохранение редактирования корректировки------------//
        [HttpPost]
        public ActionResult CorrectEditSave(int ID, int tankidECor, decimal? UrovECor, decimal? TempECor)
        {
            try
            {
                Correct corE = new Correct();
                corE = db.Correct.FirstOrDefault(s => s.id == ID);

                corE.tankid = tankidECor;
                corE.Uroven = UrovECor;
                corE.Temp = TempECor;
                db.SaveChanges();

                ViewBag.Message = "Корректировка изменена";

            }
            catch (Exception e)
            {
                ViewBag.Message = "Ошибка. Текст ошибки: " + e.ToString();
            }

            return PartialView();
        }
        //-----------------------------//

        //----------------------------------------------------------------------------------------------------------------------------------------//

    }
    }
