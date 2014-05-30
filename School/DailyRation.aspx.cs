using System;
using System.Linq;
using System.Data.Linq;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web;
using System.Collections;

public partial class School_DailyRation : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);
    private const string YELLOW = "#ffff8d";
    private const string RED = "#fd5f35";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadFoods();
            Public.LoadCycleMonths(this.drpMonths);
            this.lblCycle.Text = Public.ActiveCycle.CycleName;
            if (HttpContext.Current.User.IsInRole("SchoolManager"))
            {
                this.txtSchoolCode.Text = HttpContext.Current.User.Identity.Name;
                this.txtSchoolCode.Enabled = false;
                this.btnSchoolCodeSearch.Enabled = false;
                this.txtSchoolCode_TextChanged(sender, e);
            }

            if (System.Web.HttpContext.Current.User.IsInRole("Guest"))
            {
                this.btnSave.Enabled = false;
            }
        }
    }

    protected void txtSchoolCode_TextChanged(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(this.txtSchoolCode.Text))
        {
            if (!FillHeader())
            {
                ResetHeader();
            }
            else
            {
                this.drpMonths.Enabled = true;
                this.drpMonths.SelectedIndex = 0;
                this.drpSublevel.SelectedIndex = 0;
            }
        }
        else
        {
            ResetHeader();
        }
        ResetDays();
    }

    protected void drpMonths_SelectedIndexChanged(object sender, EventArgs e)
    {
        ResetDays();
        if (this.drpMonths.SelectedIndex > 0)
        {
            this.drpStuff.Enabled = true;
            this.drpSublevel.Enabled = true;
            string[] monthVals = this.drpMonths.SelectedValue.Split('|');
            short solarYear = Public.ToShort(monthVals[0]);
            byte solarMonth = Public.ToByte(monthVals[1]);
            SetHolidays(solarYear, solarMonth);

            if (this.drpStuff.SelectedIndex > 0)
            {
                var foodsList = from sf in db.SchoolFoods
                                join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                where sf.SchoolSubLevelID == Public.ToInt(this.drpSublevel.SelectedValue.Split('|')[0]) &&
                                         sf.CycleFoodID == Public.ToShort(this.drpStuff.SelectedValue) &&
                                         cl.SolarYear == solarYear && cl.SolarMonth == solarMonth
                                select
                                new
                                {
                                    sf.FoodCount,
                                    cl.SolarDay
                                };
                short sum = 0;
                foreach (var item in foodsList)
                {
                    SetDayRation(item.SolarDay, item.FoodCount);
                    sum += item.FoodCount;
                }
                this.txtSum.Text = sum.ToString();
            }
        }
    }

    protected void drpSublevel_SelectedIndexChanged(object sender, EventArgs e)
    {
        ResetRations();
        string[] sublevelInfo = this.drpSublevel.SelectedValue.Split('|');
        this.lblStudentCount.Text = sublevelInfo[1];
        if (!string.IsNullOrEmpty(this.hdnSchoolId.Value) && this.drpMonths.SelectedIndex > 0 && this.drpStuff.SelectedIndex > 0)
        {
            string[] monthVals = this.drpMonths.SelectedValue.Split('|');
            short solarYear = Public.ToShort(monthVals[0]);
            byte solarMonth = Public.ToByte(monthVals[1]);

            var rationsList = from sf in db.SchoolFoods
                              join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                              where sf.SchoolSubLevelID == Public.ToInt(sublevelInfo[0]) &&
                                       sf.CycleFoodID == Public.ToShort(this.drpStuff.SelectedValue) &&
                                       cl.SolarYear == solarYear && cl.SolarMonth == solarMonth
                              select
                              new
                              {
                                  sf.FoodCount,
                                  cl.SolarDay
                              };
            short sum = 0;
            foreach (var item in rationsList)
            {
                SetDayRation(item.SolarDay, item.FoodCount);
                sum += item.FoodCount;
            }
            this.txtSum.Text = sum.ToString();
        }
    }

    protected void drpStuff_SelectedIndexChanged(object sender, EventArgs e)
    {
        ResetRations();
        if (this.drpMonths.SelectedIndex > 0 && this.drpStuff.SelectedIndex > 0)
        {
            string[] sublevelInfo = this.drpSublevel.SelectedValue.Split('|');
            this.lblStudentCount.Text = sublevelInfo[1];

            string[] monthVals = this.drpMonths.SelectedValue.Split('|');
            short solarYear = Public.ToShort(monthVals[0]);
            byte solarMonth = Public.ToByte(monthVals[1]);

            var rationsList = from sf in db.SchoolFoods
                              join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                              where sf.SchoolSubLevelID == Public.ToInt(sublevelInfo[0]) &&
                                       sf.CycleFoodID == Public.ToShort(this.drpStuff.SelectedValue) &&
                                       cl.SolarYear == solarYear && cl.SolarMonth == solarMonth
                              select
                              new
                              {
                                  sf.FoodCount,
                                  cl.SolarDay
                              };
            short sum = 0;
            foreach (var item in rationsList)
            {
                SetDayRation(item.SolarDay, item.FoodCount);
                sum += item.FoodCount;
            }
            this.txtSum.Text = sum.ToString();
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (Page.IsValid && !string.IsNullOrEmpty(this.hdnSchoolId.Value) && this.drpMonths.Items.Count > 0 && this.drpStuff.SelectedIndex > 0)
        {

            int schoolSubLevelId = Public.ToInt(this.drpSublevel.SelectedValue.Split('|')[0]);
            short cycleFoodId = Public.ToShort(this.drpStuff.SelectedValue);
            string[] monthVals = this.drpMonths.SelectedValue.Split('|');
            short solarYear = Public.ToShort(monthVals[0]);
            byte solarMonth = Public.ToByte(monthVals[1]);


            //#region Ration
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<SupplySystem.SchoolFood>(sf => sf.Calendar);
            List<SupplySystem.SchoolFood> foodsList = (from sf in db.SchoolFoods
                                                       join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                       where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                                sf.CycleFoodID == cycleFoodId && cl.SolarYear == solarYear &&
                                                                cl.SolarMonth == solarMonth
                                                       select sf).ToList<SupplySystem.SchoolFood>();
            #region Ration

            short foodCount = Public.ToShort(this.txt_1.Text);
            SupplySystem.SchoolFood schFood1 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 1);
            if (schFood1 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                           {
                               SchoolSubLevelID = schoolSubLevelId,
                               CycleFoodID = cycleFoodId,
                               CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 1).CalendarID,
                               FoodCount = foodCount,
                               SubmitDate = DateTime.Now
                           });
            }
            else if (schFood1 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood1);
                }
                else if (schFood1.FoodCount != foodCount) // Edit mode
                {
                    schFood1.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_2.Text);
            SupplySystem.SchoolFood schFood2 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 2);
            if (schFood2 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 2).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood2 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood2);
                }
                else if (schFood2.FoodCount != foodCount) // Edit mode
                {
                    schFood2.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_3.Text);
            SupplySystem.SchoolFood schFood3 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 3);
            if (schFood3 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 3).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood3 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood3);
                }
                else if (schFood3.FoodCount != foodCount) // Edit mode
                {
                    schFood3.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_4.Text);
            SupplySystem.SchoolFood schFood4 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 4);
            if (schFood4 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 4).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood4 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood4);
                }
                else if (schFood4.FoodCount != foodCount) // Edit mode
                {
                    schFood4.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_5.Text);
            SupplySystem.SchoolFood schFood5 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 5);
            if (schFood5 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 5).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood5 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood5);
                }
                else if (schFood5.FoodCount != foodCount) // Edit mode
                {
                    schFood5.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_6.Text);
            SupplySystem.SchoolFood schFood6 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 6);
            if (schFood6 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 6).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood6 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood6);
                }
                else if (schFood6.FoodCount != foodCount) // Edit mode
                {
                    schFood6.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_7.Text);
            SupplySystem.SchoolFood schFood7 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 7);
            if (schFood7 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 7).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood7 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood7);
                }
                else if (schFood7.FoodCount != foodCount) // Edit mode
                {
                    schFood7.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_8.Text);
            SupplySystem.SchoolFood schFood8 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 8);
            if (schFood8 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 8).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood8 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood8);
                }
                else if (schFood8.FoodCount != foodCount) // Edit mode
                {
                    schFood8.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_9.Text);
            SupplySystem.SchoolFood schFood9 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 9);
            if (schFood9 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 9).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood9 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood9);
                }
                else if (schFood9.FoodCount != foodCount) // Edit mode
                {
                    schFood9.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_10.Text);
            SupplySystem.SchoolFood schFood10 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 10);
            if (schFood10 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 10).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood10 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood10);
                }
                else if (schFood10.FoodCount != foodCount) // Edit mode
                {
                    schFood10.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_11.Text);
            SupplySystem.SchoolFood schFood11 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 11);
            if (schFood11 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 11).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood11 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood11);
                }
                else if (schFood11.FoodCount != foodCount) // Edit mode
                {
                    schFood11.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_12.Text);
            SupplySystem.SchoolFood schFood12 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 12);
            if (schFood12 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 12).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood12 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood12);
                }
                else if (schFood12.FoodCount != foodCount) // Edit mode
                {
                    schFood12.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_13.Text);
            SupplySystem.SchoolFood schFood13 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 13);
            if (schFood13 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 13).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood13 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood13);
                }
                else if (schFood13.FoodCount != foodCount) // Edit mode
                {
                    schFood13.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_14.Text);
            SupplySystem.SchoolFood schFood14 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 14);
            if (schFood14 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 14).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood14 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood14);
                }
                else if (schFood14.FoodCount != foodCount) // Edit mode
                {
                    schFood14.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_15.Text);
            SupplySystem.SchoolFood schFood15 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 15);
            if (schFood15 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 15).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood15 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood15);
                }
                else if (schFood15.FoodCount != foodCount) // Edit mode
                {
                    schFood15.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_16.Text);
            SupplySystem.SchoolFood schFood16 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 16);
            if (schFood16 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 16).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood16 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood16);
                }
                else if (schFood16.FoodCount != foodCount) // Edit mode
                {
                    schFood16.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_17.Text);
            SupplySystem.SchoolFood schFood17 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 17);
            if (schFood17 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 17).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood17 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood17);
                }
                else if (schFood17.FoodCount != foodCount) // Edit mode
                {
                    schFood17.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_18.Text);
            SupplySystem.SchoolFood schFood18 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 18);
            if (schFood18 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 18).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood18 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood18);
                }
                else if (schFood18.FoodCount != foodCount) // Edit mode
                {
                    schFood18.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_19.Text);
            SupplySystem.SchoolFood schFood19 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 19);
            if (schFood19 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 19).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood19 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood19);
                }
                else if (schFood19.FoodCount != foodCount) // Edit mode
                {
                    schFood19.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_20.Text);
            SupplySystem.SchoolFood schFood20 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 20);
            if (schFood20 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 20).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood20 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood20);
                }
                else if (schFood20.FoodCount != foodCount) // Edit mode
                {
                    schFood20.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_21.Text);
            SupplySystem.SchoolFood schFood21 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 21);
            if (schFood21 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 21).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood21 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood21);
                }
                else if (schFood21.FoodCount != foodCount) // Edit mode
                {
                    schFood21.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_22.Text);
            SupplySystem.SchoolFood schFood22 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 22);
            if (schFood22 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 22).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood22 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood22);
                }
                else if (schFood22.FoodCount != foodCount) // Edit mode
                {
                    schFood22.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_23.Text);
            SupplySystem.SchoolFood schFood23 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 23);
            if (schFood23 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 23).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood23 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood23);
                }
                else if (schFood23.FoodCount != foodCount) // Edit mode
                {
                    schFood23.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_24.Text);
            SupplySystem.SchoolFood schFood24 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 24);
            if (schFood24 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 24).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood24 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood24);
                }
                else if (schFood24.FoodCount != foodCount) // Edit mode
                {
                    schFood24.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_25.Text);
            SupplySystem.SchoolFood schFood25 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 25);
            if (schFood25 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 25).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood25 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood25);
                }
                else if (schFood25.FoodCount != foodCount) // Edit mode
                {
                    schFood25.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_26.Text);
            SupplySystem.SchoolFood schFood26 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 26);
            if (schFood26 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 26).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood26 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood26);
                }
                else if (schFood26.FoodCount != foodCount) // Edit mode
                {
                    schFood26.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_27.Text);
            SupplySystem.SchoolFood schFood27 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 27);
            if (schFood27 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 27).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood27 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood27);
                }
                else if (schFood27.FoodCount != foodCount) // Edit mode
                {
                    schFood27.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_28.Text);
            SupplySystem.SchoolFood schFood28 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 28);
            if (schFood28 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 28).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood28 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood28);
                }
                else if (schFood28.FoodCount != foodCount) // Edit mode
                {
                    schFood28.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_29.Text);
            SupplySystem.SchoolFood schFood29 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 29);
            if (schFood29 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 29).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood29 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood29);
                }
                else if (schFood29.FoodCount != foodCount) // Edit mode
                {
                    schFood29.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_30.Text);
            SupplySystem.SchoolFood schFood30 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 30);
            if (schFood30 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 30).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood30 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood30);
                }
                else if (schFood30.FoodCount != foodCount) // Edit mode
                {
                    schFood30.FoodCount = foodCount;
                }
            }

            foodCount = Public.ToShort(this.txt_31.Text);
            SupplySystem.SchoolFood schFood31 = foodsList.FirstOrDefault<SupplySystem.SchoolFood>(sf => sf.Calendar.SolarDay == 31);
            if (schFood31 == null && foodCount > 0) // Add mode
            {
                db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                {
                    SchoolSubLevelID = schoolSubLevelId,
                    CycleFoodID = cycleFoodId,
                    CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 31).CalendarID,
                    FoodCount = foodCount,
                    SubmitDate = DateTime.Now
                });
            }
            else if (schFood31 != null)
            {
                if (foodCount == 0) // Delete mode
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood31);
                }
                else if (schFood31.FoodCount != foodCount) // Edit mode
                {
                    schFood31.FoodCount = foodCount;
                }
            }

            #endregion

            db.SubmitChanges();
            this.lblMessage.Text = Public.SUCCESSMESSAGE;
        }
    }

    private void LoadFoods()
    {
        ArrayList arrayList = HttpContext.Current.Cache["DailyFoods"] as ArrayList;
        if (arrayList == null)
        {
            arrayList = new ArrayList();
            var foods = from cf in db.CycleFoods
                        join st in db.REP_Stuffs on cf.StuffID equals st.StuffID
                        where cf.CycleID == Public.ActiveCycle.CycleID && cf.Available && cf.IsDaily
                        select new { cf.CycleFoodID, st.StuffName };
            arrayList.Add(new { CycleFoodID = 0, StuffName = "-- انتخاب کنید --" });

            foreach (var stuff in foods)
            {
                arrayList.Add(new { stuff.CycleFoodID, stuff.StuffName });
            }
            HttpContext.Current.Cache.Insert("DailyFoods", arrayList, null, DateTime.MaxValue, TimeSpan.FromMinutes(10));
        }
        this.drpStuff.DataSource = arrayList;
        this.drpStuff.DataBind();
    }

    private void SetHolidays(short solarYear, byte solarMonth)
    {
        List<SupplySystem.Calendar> calendarList = db.Calendars.Where<SupplySystem.Calendar>(c => c.SolarYear == solarYear &&
                                                                                                                                         c.SolarMonth == solarMonth).ToList<SupplySystem.Calendar>();
        if (calendarList.Count > 0)
        {
            this.td_30.Style["background-color"] = RED;
            this.txt_30.ReadOnly = true;
            this.txt_30.Text = null;
            this.td_31.Style["background-color"] = RED;
            this.txt_31.ReadOnly = true;
            this.txt_31.Text = null;

            for (int i = 0; i < calendarList.Count; i++)
            {
                switch (calendarList[i].SolarDay)
                {
                    case 1:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_1.Style["background-color"] = YELLOW;
                            this.txt_1.ReadOnly = false;
                        }
                        break;

                    case 2:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_2.Style["background-color"] = YELLOW;
                            this.txt_2.ReadOnly = false;
                        }
                        break;

                    case 3:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_3.Style["background-color"] = YELLOW;
                            this.txt_3.ReadOnly = false;
                        }
                        break;

                    case 4:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_4.Style["background-color"] = YELLOW;
                            this.txt_4.ReadOnly = false;
                        }
                        break;

                    case 5:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_5.Style["background-color"] = YELLOW;
                            this.txt_5.ReadOnly = false;
                        }
                        break;

                    case 6:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_6.Style["background-color"] = YELLOW;
                            this.txt_6.ReadOnly = false;
                        }
                        break;

                    case 7:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_7.Style["background-color"] = YELLOW;
                            this.txt_7.ReadOnly = false;
                        }
                        break;

                    case 8:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_8.Style["background-color"] = YELLOW;
                            this.txt_8.ReadOnly = false;
                        }
                        break;

                    case 9:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_9.Style["background-color"] = YELLOW;
                            this.txt_9.ReadOnly = false;
                        }
                        break;

                    case 10:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_10.Style["background-color"] = YELLOW;
                            this.txt_10.ReadOnly = false;
                        }
                        break;

                    case 11:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_11.Style["background-color"] = YELLOW;
                            this.txt_11.ReadOnly = false;
                        }
                        break;

                    case 12:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_12.Style["background-color"] = YELLOW;
                            this.txt_12.ReadOnly = false;
                        }
                        break;

                    case 13:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_13.Style["background-color"] = YELLOW;
                            this.txt_13.ReadOnly = false;
                        }
                        break;

                    case 14:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_14.Style["background-color"] = YELLOW;
                            this.txt_14.ReadOnly = false;
                        }
                        break;

                    case 15:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_15.Style["background-color"] = YELLOW;
                            this.txt_15.ReadOnly = false;
                        }
                        break;

                    case 16:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_16.Style["background-color"] = YELLOW;
                            this.txt_16.ReadOnly = false;
                        }
                        break;

                    case 17:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_17.Style["background-color"] = YELLOW;
                            this.txt_17.ReadOnly = false;
                        }
                        break;

                    case 18:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_18.Style["background-color"] = YELLOW;
                            this.txt_18.ReadOnly = false;
                        }
                        break;

                    case 19:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_19.Style["background-color"] = YELLOW;
                            this.txt_19.ReadOnly = false;
                        }
                        break;

                    case 20:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_20.Style["background-color"] = YELLOW;
                            this.txt_20.ReadOnly = false;
                        }
                        break;

                    case 21:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_21.Style["background-color"] = YELLOW;
                            this.txt_21.ReadOnly = false;
                        }
                        break;

                    case 22:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_22.Style["background-color"] = YELLOW;
                            this.txt_22.ReadOnly = false;
                        }
                        break;

                    case 23:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_23.Style["background-color"] = YELLOW;
                            this.txt_23.ReadOnly = false;
                        }
                        break;

                    case 24:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_24.Style["background-color"] = YELLOW;
                            this.txt_24.ReadOnly = false;
                        }
                        break;

                    case 25:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_25.Style["background-color"] = YELLOW;
                            this.txt_25.ReadOnly = false;
                        }
                        break;

                    case 26:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_26.Style["background-color"] = YELLOW;
                            this.txt_26.ReadOnly = false;
                        }
                        break;

                    case 27:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_27.Style["background-color"] = YELLOW;
                            this.txt_27.ReadOnly = false;
                        }
                        break;

                    case 28:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_28.Style["background-color"] = YELLOW;
                            this.txt_28.ReadOnly = false;
                        }
                        break;

                    case 29:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_29.Style["background-color"] = YELLOW;
                            this.txt_29.ReadOnly = false;
                        }
                        break;

                    case 30:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_30.Style["background-color"] = YELLOW;
                            this.txt_30.ReadOnly = false;
                        }
                        break;

                    case 31:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.td_31.Style["background-color"] = YELLOW;
                            this.txt_31.ReadOnly = false;
                        }
                        break;
                }
            }
        }
    }

    private bool FillHeader()
    {
        bool result = false;
        var school = from s in db.Schools
                     join ar in db.Areas on s.AreaCode equals ar.AreaCode
                     join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                     join lv in db.Levels on slv.LevelID equals lv.LevelID
                     where s.SchoolCode == Public.ToInt(this.txtSchoolCode.Text) && slv.LockOutDate == null
                     select
                     new
                     {
                         s.SchoolID,
                         s.SchoolCode,
                         s.SchoolName,
                         s.Gender,
                         lv.LevelName,
                         ar.AreaCode,
                         EmployeesCount = s.EmployeesCount_Fixed.GetValueOrDefault() + s.EmployeesCount_Changable.GetValueOrDefault()
                     };

        if (System.Web.HttpContext.Current.User.IsInRole("AreaManager"))
        {
            school = from q in school
                     where q.AreaCode == (from u in db.Users join ur in db.UsersInRoles on u.UserID equals ur.UserID where u.UserName.Equals(HttpContext.Current.User.Identity.Name) select ur.AreaCode).SingleOrDefault()
                     select q;
        }
        else if (System.Web.HttpContext.Current.User.IsInRole("SchoolManager"))
        {
            school = from q in school
                     where q.SchoolCode == Public.ToInt(HttpContext.Current.User.Identity.Name)
                     select q;
        }

        int employeesCount = 0;
        foreach (var item in school)
        {
            this.hdnSchoolId.Value = item.SchoolID.ToString();
            this.txtSchoolName.Text = item.SchoolName;
            this.lblLevel.Text = item.LevelName;
            this.lblGender.Text = Public.GetGender(item.Gender);
            employeesCount = item.EmployeesCount;

            var subLevels = from slv in db.SchoolLevels
                            join ssl in db.SchoolSubLevels on slv.SchoolLevelID equals ssl.SchoolLevelID
                            join sl in db.SubLevels on ssl.SubLevelID equals sl.SubLevelID
                            where slv.SchoolID == Public.ToInt(this.hdnSchoolId.Value) && slv.LockOutDate == null
                            select
                            new
                            {
                                ssl.SchoolSubLevelID,
                                sl.SubLevelName,
                                StudentCount = ssl.BoysCount.GetValueOrDefault() + ssl.GirlsCount.GetValueOrDefault()
                            };

            this.drpSublevel.Items.Clear();
            foreach (var sl in subLevels)
            {
                if (sl.SubLevelName.Equals("Employees"))
                {
                    this.drpSublevel.Items.Add(new ListItem("پرسنل", string.Concat(sl.SchoolSubLevelID, "|", employeesCount)));
                }
                else
                {
                    this.drpSublevel.Items.Add(new ListItem(sl.SubLevelName, string.Concat(sl.SchoolSubLevelID, "|", sl.StudentCount)));
                }
            }
            this.lblStudentCount.Text = this.drpSublevel.SelectedValue.Split('|')[1];
            result = true;
        }

        return result;
    }

    private void ResetHeader()
    {
        this.hdnSchoolId.Value = null;
        this.drpMonths.SelectedIndex = 0;
        this.drpMonths.Enabled = false;
        this.drpSublevel.Items.Clear();
        this.drpSublevel.Enabled = false;
        this.lblLevel.Text = null;
        this.lblGender.Text = null;
        this.txtSchoolName.Text = null;
        this.lblStudentCount.Text = null;
        this.drpStuff.SelectedIndex = 0;
    }

    private void ResetDays()
    {
        this.td_1.Style["background-color"] = RED;
        this.txt_1.ReadOnly = true;
        this.txt_1.Text = null;
        this.td_2.Style["background-color"] = RED;
        this.txt_2.ReadOnly = true;
        this.txt_2.Text = null;
        this.td_3.Style["background-color"] = RED;
        this.txt_3.ReadOnly = true;
        this.txt_3.Text = null;
        this.td_4.Style["background-color"] = RED;
        this.txt_4.ReadOnly = true;
        this.txt_4.Text = null;
        this.td_5.Style["background-color"] = RED;
        this.txt_5.ReadOnly = true;
        this.txt_5.Text = null;
        this.td_6.Style["background-color"] = RED;
        this.txt_6.ReadOnly = true;
        this.txt_6.Text = null;
        this.td_7.Style["background-color"] = RED;
        this.txt_7.ReadOnly = true;
        this.txt_7.Text = null;
        this.td_8.Style["background-color"] = RED;
        this.txt_8.ReadOnly = true;
        this.txt_8.Text = null;
        this.td_9.Style["background-color"] = RED;
        this.txt_9.ReadOnly = true;
        this.txt_9.Text = null;
        this.td_10.Style["background-color"] = RED;
        this.txt_10.ReadOnly = true;
        this.txt_10.Text = null;
        this.td_11.Style["background-color"] = RED;
        this.txt_11.ReadOnly = true;
        this.txt_11.Text = null;
        this.td_12.Style["background-color"] = RED;
        this.txt_12.ReadOnly = true;
        this.txt_12.Text = null;
        this.td_13.Style["background-color"] = RED;
        this.txt_13.ReadOnly = true;
        this.txt_13.Text = null;
        this.td_14.Style["background-color"] = RED;
        this.txt_14.ReadOnly = true;
        this.txt_14.Text = null;
        this.td_15.Style["background-color"] = RED;
        this.txt_15.ReadOnly = true;
        this.txt_15.Text = null;
        this.td_16.Style["background-color"] = RED;
        this.txt_16.ReadOnly = true;
        this.txt_16.Text = null;
        this.td_17.Style["background-color"] = RED;
        this.txt_17.ReadOnly = true;
        this.txt_17.Text = null;
        this.td_18.Style["background-color"] = RED;
        this.txt_18.ReadOnly = true;
        this.txt_18.Text = null;
        this.td_19.Style["background-color"] = RED;
        this.txt_19.ReadOnly = true;
        this.txt_19.Text = null;
        this.td_20.Style["background-color"] = RED;
        this.txt_20.ReadOnly = true;
        this.txt_20.Text = null;
        this.td_21.Style["background-color"] = RED;
        this.txt_21.ReadOnly = true;
        this.txt_21.Text = null;
        this.td_22.Style["background-color"] = RED;
        this.txt_22.ReadOnly = true;
        this.txt_22.Text = null;
        this.td_23.Style["background-color"] = RED;
        this.txt_23.ReadOnly = true;
        this.txt_23.Text = null;
        this.td_24.Style["background-color"] = RED;
        this.txt_24.ReadOnly = true;
        this.txt_24.Text = null;
        this.td_25.Style["background-color"] = RED;
        this.txt_25.ReadOnly = true;
        this.txt_25.Text = null;
        this.td_26.Style["background-color"] = RED;
        this.txt_26.ReadOnly = true;
        this.txt_26.Text = null;
        this.td_27.Style["background-color"] = RED;
        this.txt_27.ReadOnly = true;
        this.txt_27.Text = null;
        this.td_28.Style["background-color"] = RED;
        this.txt_28.ReadOnly = true;
        this.txt_28.Text = null;
        this.td_29.Style["background-color"] = RED;
        this.txt_29.ReadOnly = true;
        this.txt_29.Text = null;
        this.td_30.Style["background-color"] = RED;
        this.txt_30.ReadOnly = true;
        this.txt_30.Text = null;
        this.td_31.Style["background-color"] = RED;
        this.txt_31.ReadOnly = true;
        this.txt_31.Text = null;
    }

    private void ResetRations()
    {
        this.txt_1.Text = null;
        this.txt_2.Text = null;
        this.txt_3.Text = null;
        this.txt_4.Text = null;
        this.txt_5.Text = null;
        this.txt_6.Text = null;
        this.txt_7.Text = null;
        this.txt_8.Text = null;
        this.txt_9.Text = null;
        this.txt_10.Text = null;
        this.txt_11.Text = null;
        this.txt_12.Text = null;
        this.txt_13.Text = null;
        this.txt_14.Text = null;
        this.txt_15.Text = null;
        this.txt_16.Text = null;
        this.txt_17.Text = null;
        this.txt_18.Text = null;
        this.txt_19.Text = null;
        this.txt_20.Text = null;
        this.txt_21.Text = null;
        this.txt_22.Text = null;
        this.txt_23.Text = null;
        this.txt_24.Text = null;
        this.txt_25.Text = null;
        this.txt_26.Text = null;
        this.txt_27.Text = null;
        this.txt_28.Text = null;
        this.txt_29.Text = null;
        this.txt_30.Text = null;
        this.txt_31.Text = null;
        this.txtSum.Text = null;
    }

    private void SetDayRation(byte dayIndex, short ration)
    {
        switch (dayIndex)
        {
            case 1:
                this.txt_1.Text = ration.ToString();
                break;

            case 2:
                this.txt_2.Text = ration.ToString();
                break;

            case 3:
                this.txt_3.Text = ration.ToString();
                break;

            case 4:
                this.txt_4.Text = ration.ToString();
                break;

            case 5:
                this.txt_5.Text = ration.ToString();
                break;

            case 6:
                this.txt_6.Text = ration.ToString();
                break;

            case 7:
                this.txt_7.Text = ration.ToString();
                break;

            case 8:
                this.txt_8.Text = ration.ToString();
                break;

            case 9:
                this.txt_9.Text = ration.ToString();
                break;

            case 10:
                this.txt_10.Text = ration.ToString();
                break;

            case 11:
                this.txt_11.Text = ration.ToString();
                break;

            case 12:
                this.txt_12.Text = ration.ToString();
                break;

            case 13:
                this.txt_13.Text = ration.ToString();
                break;

            case 14:
                this.txt_14.Text = ration.ToString();
                break;

            case 15:
                this.txt_15.Text = ration.ToString();
                break;

            case 16:
                this.txt_16.Text = ration.ToString();
                break;

            case 17:
                this.txt_17.Text = ration.ToString();
                break;

            case 18:
                this.txt_18.Text = ration.ToString();
                break;

            case 19:
                this.txt_19.Text = ration.ToString();
                break;

            case 20:
                this.txt_20.Text = ration.ToString();
                break;

            case 21:
                this.txt_21.Text = ration.ToString();
                break;

            case 22:
                this.txt_22.Text = ration.ToString();
                break;

            case 23:
                this.txt_23.Text = ration.ToString();
                break;

            case 24:
                this.txt_24.Text = ration.ToString();
                break;

            case 25:
                this.txt_25.Text = ration.ToString();
                break;

            case 26:
                this.txt_26.Text = ration.ToString();
                break;

            case 27:
                this.txt_27.Text = ration.ToString();
                break;

            case 28:
                this.txt_28.Text = ration.ToString();
                break;

            case 29:
                this.txt_29.Text = ration.ToString();
                break;

            case 30:
                this.txt_30.Text = ration.ToString();
                break;

            case 31:
                this.txt_31.Text = ration.ToString();
                break;
        }
    }
}