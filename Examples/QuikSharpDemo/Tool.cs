using System;
using QuikSharp;
using QuikSharp.DataStructures;
using QuikSharp.Quik;

public class Tool   
{
    readonly Char separator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
    readonly IQuik _quik;
    string name;
    string securityCode;
    string classCode;
    //string clientCode;
    string accountID;
    string firmID;
    int lot;
    int priceAccuracy;
    double guaranteeProviding;
    decimal step;
    decimal lastPrice;

    #region Свойства
    /// <summary>
    /// Краткое наименование инструмента (бумаги)
    /// </summary>
    public string Name { get { return name; } }
    /// <summary>
    /// Код инструмента (бумаги)
    /// </summary>
    public string SecurityCode { get { return securityCode; } }
    /// <summary>
    /// Код класса инструмента (бумаги)
    /// </summary>
    public string ClassCode { get { return classCode; } }
    /// <summary>
    /// Счет клиента
    /// </summary>
    public string AccountID { get { return accountID; } }
    /// <summary>
    /// Код фирмы
    /// </summary>
    public string FirmID { get { return firmID; } }
    /// <summary>
    /// Количество акций в одном лоте
    /// Для инструментов класса SPBFUT = 1
    /// </summary>
    public int Lot { get { return lot; } }
    /// <summary>
    /// Точность цены (количество знаков после запятой)
    /// </summary>
    public int PriceAccuracy { get { return priceAccuracy; } }
    /// <summary>
    /// Шаг цены
    /// </summary>
    public decimal Step { get { return step; } }
    /// <summary>
    /// Гарантийное обеспечение (только для срочного рынка)
    /// для фондовой секции = 0
    /// </summary>
    public double GuaranteeProviding { get { return guaranteeProviding; } }
    /// <summary>
    /// Цена последней сделки
    /// </summary>
    public decimal LastPrice
    {
        get
        {
            lastPrice = Convert.ToDecimal(_quik.Functions.Trading.GetParamEx(classCode, securityCode, "LAST").Result.ParamValue.Replace('.', separator));
            return lastPrice;
        }
    }
    #endregion

    /// <summary>
    /// Конструктор класса
    /// </summary>
    /// <param name="_quik"></param>
    /// <param name="securityCode">Код инструмента</param>
    /// <param name="classCode">Код класса</param>
    public Tool(IQuik quik, string securityCode_, string _classCode)
    {
        _quik = quik;
        GetBaseParam(quik, securityCode_, _classCode);
    }

    void GetBaseParam(IQuik quik, string secCode, string _classCode)
    {
        try
        {
            securityCode = secCode;
            classCode = _classCode;
            if (quik != null)
            {
                if (classCode != null && classCode != "")
                {
                    try
                    {
                        name = quik.Functions.Class.GetSecurityInfo(classCode, securityCode).Result.ShortName;
                        accountID = quik.Functions.Class.GetTradeAccount(classCode).Result;
                        firmID = quik.Functions.Class.GetClassInfo(classCode).Result.FirmId;
                        //step = Convert.ToDecimal(quik.Trading.GetParamEx(classCode, securityCode, "SEC_PRICE_STEP").Result.ParamValue.Replace('.', separator));
                        //priceAccuracy = Convert.ToInt32(Convert.ToDouble(quik.Trading.GetParamEx(classCode, securityCode, "SEC_SCALE").Result.ParamValue.Replace('.', separator)));
                        step = Convert.ToDecimal(quik.Functions.Trading.GetParamEx(classCode, securityCode, ParamName.SEC_PRICE_STEP).Result.ParamValue.Replace('.', separator));
                        priceAccuracy = Convert.ToInt32(Convert.ToDouble(quik.Functions.Trading.GetParamEx(classCode, securityCode, ParamName.SEC_SCALE).Result.ParamValue.Replace('.', separator)));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Tool.GetBaseParam. Ошибка получения наименования для " + securityCode + ": " + e.Message);
                    }

                    if (classCode == "SPBFUT")
                    {
                        Console.WriteLine("Получаем 'guaranteeProviding'.");
                        lot = 1;
                        //guaranteeProviding = Convert.ToDouble(quik.Trading.GetParamEx(classCode, securityCode, "BUYDEPO").Result.ParamValue.Replace('.', separator));
                        guaranteeProviding = Convert.ToDouble(quik.Functions.Trading.GetParamEx(classCode, securityCode, ParamName.BUYDEPO).Result.ParamValue.Replace('.', separator));
                    }
                    else
                    {
                        Console.WriteLine("Получаем 'lot'.");
                        //lot = Convert.ToInt32(Convert.ToDouble(quik.Trading.GetParamEx(classCode, securityCode, "LOTSIZE").Result.ParamValue.Replace('.', separator)));
                        lot = Convert.ToInt32(Convert.ToDouble(quik.Functions.Trading.GetParamEx(classCode, securityCode, ParamName.LOTSIZE).Result.ParamValue.Replace('.', separator)));
                        guaranteeProviding = 0;
                    }
                }
                else
                {
                    Console.WriteLine("Tool.GetBaseParam. Ошибка: classCode не определен.");
                    lot = 0;
                    guaranteeProviding = 0;
                }
            }
            else
            {
                Console.WriteLine("Tool.GetBaseParam. quik = null !");
            }
        }
        catch (NullReferenceException e)
        {
            Console.WriteLine("Ошибка NullReferenceException в методе GetBaseParam: " + e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка в методе GetBaseParam: " + e.Message);
        }
    }
}
