using System;
using System.Collections.Generic;
using DormAPI.Database.EF.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace DormAPI.Database.EF.Contexts;

public partial class DormContext : DbContext
{
    public DormContext()
    {
    }

    public DormContext(DbContextOptions<DormContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Building> Buildings { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<DocumentsPath> DocumentsPaths { get; set; }

    public virtual DbSet<EducationLevel> EducationLevels { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeLeaf> EmployeeLeaves { get; set; }

    public virtual DbSet<EmployeeType> EmployeeTypes { get; set; }

    public virtual DbSet<EmployeesContract> EmployeesContracts { get; set; }

    public virtual DbSet<Expense> Expenses { get; set; }

    public virtual DbSet<ExpenseCategory> ExpenseCategories { get; set; }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<Floor> Floors { get; set; }

    public virtual DbSet<Income> Incomes { get; set; }

    public virtual DbSet<IncomeCategory> IncomeCategories { get; set; }

    public virtual DbSet<LeaveType> LeaveTypes { get; set; }

    public virtual DbSet<Nationality> Nationalities { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<RelationshipType> RelationshipTypes { get; set; }

    public virtual DbSet<RentInstallmentsLedger> RentInstallmentsLedgers { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomAllocation> RoomAllocations { get; set; }

    public virtual DbSet<RoomStatus> RoomStatuses { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<StatusType> StatusTypes { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentDeposit> StudentDeposits { get; set; }

    public virtual DbSet<University> Universities { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<ViewBuilding> ViewBuildings { get; set; }

    public virtual DbSet<ViewBuildingDetail> ViewBuildingDetails { get; set; }

    public virtual DbSet<ViewBuildingsStatistic> ViewBuildingsStatistics { get; set; }

    public virtual DbSet<ViewEmployee> ViewEmployees { get; set; }

    public virtual DbSet<ViewEmployeeLeaf> ViewEmployeeLeaves { get; set; }

    public virtual DbSet<ViewEmployeesContract> ViewEmployeesContracts { get; set; }

    public virtual DbSet<ViewExpenseOverview> ViewExpenseOverviews { get; set; }

    public virtual DbSet<ViewIcomeOverview> ViewIcomeOverviews { get; set; }

    public virtual DbSet<ViewIcomesTotalOverview> ViewIcomesTotalOverviews { get; set; }

    public virtual DbSet<ViewPaymentsIcomeOverview> ViewPaymentsIcomeOverviews { get; set; }

    public virtual DbSet<ViewStudent> ViewStudents { get; set; }

    public virtual DbSet<WorkingDay> WorkingDays { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=Dorm;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Building>(entity =>
        {
            entity.HasKey(e => e.BuildingId).HasName("PK__Building__5463CDE4C8BC08AA");

            entity.ToTable("Building");

            entity.Property(e => e.BuildingId).HasColumnName("BuildingID");
            entity.Property(e => e.BuildingName).HasMaxLength(100);
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("PK__Contacts__5C6625BBE66B5287");

            entity.HasIndex(e => e.PhoneNumber, "UQ__Contacts__85FB4E3889781058").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Contacts__A9D10534FAAF4CD4").IsUnique();

            entity.Property(e => e.ContactId).HasColumnName("ContactID");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EmergencyContactName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EmergencyContactPhone)
                .HasMaxLength(13)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(13)
                .IsUnicode(false);
            entity.Property(e => e.RelationshipTypeId).HasColumnName("RelationshipTypeID");

            entity.HasOne(d => d.RelationshipType).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.RelationshipTypeId)
                .HasConstraintName("FK_Foreign_Contacts_RelationshipTypes");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__Contract__C90D34093746AB49");

            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.AdditionalFees).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ContractName).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Discount).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MonthlyAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Student).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Contracts__Stude__6E01572D");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__B2079BCD21B16CA9");

            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.DepartmentName).HasMaxLength(100);
        });

        modelBuilder.Entity<DocumentsPath>(entity =>
        {
            entity.HasKey(e => e.DocumentPathId).HasName("PK__Document__15B6EABD1B625655");

            entity.ToTable("DocumentsPath");

            entity.Property(e => e.DocumentPathId).HasColumnName("DocumentPathID");
            entity.Property(e => e.DocumentName).HasMaxLength(100);
            entity.Property(e => e.DocumentPath).HasMaxLength(300);
            entity.Property(e => e.DocumentType).HasMaxLength(100);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Student).WithMany(p => p.DocumentsPaths)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__Documents__Stude__1F63A897");
        });

        modelBuilder.Entity<EducationLevel>(entity =>
        {
            entity.HasKey(e => e.LevelId).HasName("PK__Educatio__09F03C06E06CD83F");

            entity.ToTable("EducationLevel");

            entity.HasIndex(e => e.LevelName, "UQ__Educatio__9EF3BE7BFC561F9F").IsUnique();

            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.LevelName).HasMaxLength(50);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF13255C4C9");

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.EmployeeTypeId).HasColumnName("EmployeeTypeID");
            entity.Property(e => e.PersonId).HasColumnName("PersonID");

            entity.HasOne(d => d.EmployeeType).WithMany(p => p.Employees)
                .HasForeignKey(d => d.EmployeeTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employees__Emplo__15A53433");

            entity.HasOne(d => d.Person).WithMany(p => p.Employees)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employees__Perso__1699586C");
        });

        modelBuilder.Entity<EmployeeLeaf>(entity =>
        {
            entity.HasKey(e => e.LeaveId).HasName("PK__Employee__796DB95900A78854");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.StatusId).HasDefaultValue(1);

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeLeaves)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployeeL__Emplo__4FD1D5C8");

            entity.HasOne(d => d.LeaveType).WithMany(p => p.EmployeeLeaves)
                .HasForeignKey(d => d.LeaveTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployeeL__Leave__50C5FA01");

            entity.HasOne(d => d.Status).WithMany(p => p.EmployeeLeaves)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__EmployeeL__Statu__51BA1E3A");
        });

        modelBuilder.Entity<EmployeeType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__Employee__516F03952E54C630");

            entity.Property(e => e.TypeId).HasColumnName("TypeID");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TypeName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EmployeesContract>(entity =>
        {
            entity.HasKey(e => e.EmployeeContractId).HasName("PK__Employee__E17E04F6C9AD0BB2");

            entity.HasOne(d => d.AssignedBuilding).WithMany(p => p.EmployeesContracts)
                .HasForeignKey(d => d.AssignedBuildingId)
                .HasConstraintName("FK__Employees__Assig__324172E1");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeesContracts)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Employees__Assig__30592A6F");

            entity.HasOne(d => d.Shift).WithMany(p => p.EmployeesContracts)
                .HasForeignKey(d => d.ShiftId)
                .HasConstraintName("FK__Employees__Shift__314D4EA8");
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.ExpenseId).HasName("PK__Expenses__1445CFF3FF6A3FBE");

            entity.Property(e => e.ExpenseId).HasColumnName("ExpenseID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpenseAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ExpenseGategoryId).HasColumnName("ExpenseGategoryID");

            entity.HasOne(d => d.ExpenseGategory).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.ExpenseGategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Expenses__Expens__6ABAD62E");
        });

        modelBuilder.Entity<ExpenseCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__ExpenseC__19093A2B8481FD69");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(255);
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(e => e.FacultyId).HasName("PK__Facultie__306F636EED7D2084");

            entity.Property(e => e.FacultyId).HasColumnName("FacultyID");
            entity.Property(e => e.FacultyCode).HasMaxLength(10);
            entity.Property(e => e.FacultyName).HasMaxLength(100);
        });

        modelBuilder.Entity<Floor>(entity =>
        {
            entity.HasKey(e => e.FloorId).HasName("PK__Floor__49D1E86BAF58602A");

            entity.ToTable("Floor");

            entity.HasIndex(e => new { e.BuildingId, e.FloorNumber }, "Ux_Floor_Building_FloorNumber").IsUnique();

            entity.Property(e => e.FloorId).HasColumnName("FloorID");
            entity.Property(e => e.BuildingId).HasColumnName("BuildingID");

            entity.HasOne(d => d.Building).WithMany(p => p.Floors)
                .HasForeignKey(d => d.BuildingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Floor__BuildingI__619B8048");
        });

        modelBuilder.Entity<Income>(entity =>
        {
            entity.HasKey(e => e.IncomeId).HasName("PK__Incomes__60DFC66C801346D9");

            entity.Property(e => e.IncomeId).HasColumnName("IncomeID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IncomeAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IncomeCategoryId).HasColumnName("IncomeCategoryID");

            entity.HasOne(d => d.IncomeCategory).WithMany(p => p.Incomes)
                .HasForeignKey(d => d.IncomeCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Incomes__IncomeC__74444068");
        });

        modelBuilder.Entity<IncomeCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__IncomeCa__19093A2B001D4644");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(255);
        });

        modelBuilder.Entity<LeaveType>(entity =>
        {
            entity.HasKey(e => e.LeaveTypeId).HasName("PK__LeaveTyp__43BE8FF439D0317F");

            entity.Property(e => e.LeaveTypeId).HasColumnName("LeaveTypeID");
            entity.Property(e => e.LeaveTypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<Nationality>(entity =>
        {
            entity.HasKey(e => e.NationalityId).HasName("PK__National__F628E7A4C5F42B72");

            entity.Property(e => e.NationalityId).HasColumnName("NationalityID");
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.Nationality1)
                .HasMaxLength(100)
                .HasColumnName("Nationality");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A58E67B39BB");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.Notes).HasMaxLength(250);
            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.RentInstallmentId).HasColumnName("RentInstallmentID");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentMethodId)
                .HasConstraintName("FK__Payments__Paymen__17C286CF");

            entity.HasOne(d => d.RentInstallment).WithMany(p => p.Payments)
                .HasForeignKey(d => d.RentInstallmentId)
                .HasConstraintName("FK__Payments__RentIn__16CE6296");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PK__PaymentM__DC31C1F34DFED5B5");

            entity.ToTable("PaymentMethod");

            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.PaymentMethodName).HasMaxLength(50);
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.PersonId).HasName("PK__Persons__AA2FFB857843883A");

            entity.Property(e => e.PersonId).HasColumnName("PersonID");
            entity.Property(e => e.ContactId).HasColumnName("ContactID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdentityNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NationalityId).HasColumnName("NationalityID");
            entity.Property(e => e.PassportNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SecondName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ThirdName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Contact).WithMany(p => p.People)
                .HasForeignKey(d => d.ContactId)
                .HasConstraintName("FK_Foreign_Persons_Contacts");

            entity.HasOne(d => d.Nationality).WithMany(p => p.People)
                .HasForeignKey(d => d.NationalityId)
                .HasConstraintName("FK_Foreign_Persons_Nationalities");
        });

        modelBuilder.Entity<RelationshipType>(entity =>
        {
            entity.HasKey(e => e.RelationshipTypeId).HasName("PK__Relation__20FE5F612111D679");

            entity.Property(e => e.RelationshipTypeId).HasColumnName("RelationshipTypeID");
            entity.Property(e => e.RelationshipName).HasMaxLength(50);
        });

        modelBuilder.Entity<RentInstallmentsLedger>(entity =>
        {
            entity.HasKey(e => e.RentInstallmentId).HasName("PK__RentInst__77C28E20685CF99E");

            entity.ToTable("RentInstallmentsLedger");

            entity.Property(e => e.RentInstallmentId).HasColumnName("RentInstallmentID");
            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.InstallmentAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");

            entity.HasOne(d => d.Contract).WithMany(p => p.RentInstallmentsLedgers)
                .HasForeignKey(d => d.ContractId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RentInsta__Contr__70DDC3D8");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.RentInstallmentsLedgers)
                .HasForeignKey(d => d.PaymentMethodId)
                .HasConstraintName("FK_RentInstallmentsLedger_PaymentMethod");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__Room__32863919B3C462FF");

            entity.ToTable("Room");

            entity.Property(e => e.RoomId).HasColumnName("RoomID");
            entity.Property(e => e.FloorId).HasColumnName("FloorID");
            entity.Property(e => e.RoomNumber).HasMaxLength(50);
            entity.Property(e => e.RoomStatusId).HasColumnName("RoomStatusID");

            entity.HasOne(d => d.Floor).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.FloorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Room_Floor");

            entity.HasOne(d => d.RoomStatus).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.RoomStatusId)
                .HasConstraintName("FK_Room_RoomStatus");
        });

        modelBuilder.Entity<RoomAllocation>(entity =>
        {
            entity.HasKey(e => e.RoomAllocationId).HasName("PK__RoomAllo__3D96B811EAFACE10");

            entity.ToTable("RoomAllocation");

            entity.Property(e => e.RoomAllocationId).HasColumnName("RoomAllocationID");
            entity.Property(e => e.RoomId).HasColumnName("RoomID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Room).WithMany(p => p.RoomAllocations)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK__RoomAlloc__RoomI__607251E5");

            entity.HasOne(d => d.Student).WithMany(p => p.RoomAllocations)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__RoomAlloc__Stude__6166761E");
        });

        modelBuilder.Entity<RoomStatus>(entity =>
        {
            entity.HasKey(e => e.RoomStatusId).HasName("PK__RoomStat__D29DF53644BED56E");

            entity.ToTable("RoomStatus");

            entity.Property(e => e.RoomStatusId).HasColumnName("RoomStatusID");
            entity.Property(e => e.RoomStatus1)
                .HasMaxLength(100)
                .HasColumnName("RoomStatus");
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(e => e.ShiftId).HasName("PK__Shifts__C0A8388122817070");

            entity.Property(e => e.ShiftName).HasMaxLength(50);
        });

        modelBuilder.Entity<StatusType>(entity =>
        {
            entity.HasKey(e => e.StatusTypeId).HasName("PK__StatusTy__A84F3D93AFEFE639");

            entity.Property(e => e.StatusTypeId).HasColumnName("StatusTypeID");
            entity.Property(e => e.StatusTypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__32C52A79E95FE455");

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.EducationLevelId).HasColumnName("EducationLevelID");
            entity.Property(e => e.FacultyId).HasColumnName("FacultyID");
            entity.Property(e => e.PersonId).HasColumnName("PersonID");
            entity.Property(e => e.RoomAllocationId).HasColumnName("RoomAllocationID");
            entity.Property(e => e.StudentNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UniversityId).HasColumnName("UniversityID");

            entity.HasOne(d => d.Department).WithMany(p => p.Students)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Students_Departments");

            entity.HasOne(d => d.EducationLevel).WithMany(p => p.Students)
                .HasForeignKey(d => d.EducationLevelId)
                .HasConstraintName("FK_Students_EducationLevel");

            entity.HasOne(d => d.Faculty).WithMany(p => p.Students)
                .HasForeignKey(d => d.FacultyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Foreign_Students_Faculties");

            entity.HasOne(d => d.Person).WithMany(p => p.Students)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Foreign_Students_People");

            entity.HasOne(d => d.RoomAllocation).WithMany(p => p.Students)
                .HasForeignKey(d => d.RoomAllocationId)
                .HasConstraintName("FK_Students_RoomAllocation");

            entity.HasOne(d => d.University).WithMany(p => p.Students)
                .HasForeignKey(d => d.UniversityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Foreign_Students_Universities");
        });

        modelBuilder.Entity<StudentDeposit>(entity =>
        {
            entity.HasKey(e => e.DepositId).HasName("PK__StudentD__AB60DF513A051803");

            entity.ToTable("StudentDeposit");

            entity.Property(e => e.DepositId).HasColumnName("DepositID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DepositAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsRefunded)
                .HasDefaultValue(false)
                .HasColumnName("isRefunded");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentDeposits)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentDeposit_Student");
        });

        modelBuilder.Entity<University>(entity =>
        {
            entity.HasKey(e => e.UniversityId).HasName("PK__universi__F24BB7201B848744");

            entity.ToTable("universities");

            entity.Property(e => e.UniversityId).HasColumnName("university_id");
            entity.Property(e => e.UniversityName)
                .HasMaxLength(255)
                .HasColumnName("university_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC1EC72EBD");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PersonId).HasColumnName("PersonID");
            entity.Property(e => e.UserRoleId).HasColumnName("UserRoleID");

            entity.HasOne(d => d.Person).WithMany(p => p.Users)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__PersonID__5BE2A6F2");

            entity.HasOne(d => d.UserRole).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__UserRoleI__5CD6CB2B");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("PK__UserRole__3D978A55E9ACF12F");

            entity.Property(e => e.UserRoleId).HasColumnName("UserRoleID");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.UserRoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ViewBuilding>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ViewBuildings");

            entity.Property(e => e.BuildingId).HasColumnName("BuildingID");
            entity.Property(e => e.BuildingName).HasMaxLength(100);
            entity.Property(e => e.FloorCount).HasColumnName("floor_count");
            entity.Property(e => e.RoomCount).HasColumnName("room_count");
        });

        modelBuilder.Entity<ViewBuildingDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ViewBuildingDetails");

            entity.Property(e => e.BuildingId).HasColumnName("BuildingID");
            entity.Property(e => e.BuildingName).HasMaxLength(100);
        });

        modelBuilder.Entity<ViewBuildingsStatistic>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ViewBuildingsStatistics");
        });

        modelBuilder.Entity<ViewEmployee>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ViewEmployees");

            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ContactId).HasColumnName("ContactID");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EmergencyContactName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EmergencyContactPhone)
                .HasMaxLength(13)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdentityNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NationalityId).HasColumnName("NationalityID");
            entity.Property(e => e.PassportNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PersonId).HasColumnName("PersonID");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(13)
                .IsUnicode(false);
            entity.Property(e => e.RelationshipName).HasMaxLength(50);
            entity.Property(e => e.RelationshipTypeId).HasColumnName("RelationshipTypeID");
            entity.Property(e => e.SecondName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ThirdName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TypeId).HasColumnName("TypeID");
            entity.Property(e => e.TypeName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ViewEmployeeLeaf>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ViewEmployeeLeaves");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(101)
                .IsUnicode(false);
            entity.Property(e => e.LeaveTypeName).HasMaxLength(100);
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.StatusTypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<ViewEmployeesContract>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ViewEmployeesContracts");

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
        });

        modelBuilder.Entity<ViewExpenseOverview>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ViewExpenseOverview");

            entity.Property(e => e.CategoryName).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ExpenseAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ExpenseGategoryId).HasColumnName("ExpenseGategoryID");
            entity.Property(e => e.ExpenseId).HasColumnName("ExpenseID");
        });

        modelBuilder.Entity<ViewIcomeOverview>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ViewIcomeOverview");

            entity.Property(e => e.IncomeAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IncomeCategoryId).HasColumnName("IncomeCategoryID");
            entity.Property(e => e.IncomeId).HasColumnName("IncomeID");
        });

        modelBuilder.Entity<ViewIcomesTotalOverview>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ViewIcomesTotalOverview");

            entity.Property(e => e.CategoryName).HasMaxLength(255);
            entity.Property(e => e.IncomeAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IncomeCategoryId).HasColumnName("IncomeCategoryID");
            entity.Property(e => e.IncomeId).HasColumnName("IncomeID");
        });

        modelBuilder.Entity<ViewPaymentsIcomeOverview>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ViewPaymentsIcomeOverview");

            entity.Property(e => e.IncomeCategoryId).HasColumnName("IncomeCategoryID");
            entity.Property(e => e.InstallmentAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
        });

        modelBuilder.Entity<ViewStudent>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ViewStudents");

            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ContactId).HasColumnName("ContactID");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.DepartmentName).HasMaxLength(100);
            entity.Property(e => e.EducationLevelId).HasColumnName("EducationLevelID");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EmergencyContactName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EmergencyContactPhone)
                .HasMaxLength(13)
                .IsUnicode(false);
            entity.Property(e => e.FacultyId).HasColumnName("FacultyID");
            entity.Property(e => e.FacultyName).HasMaxLength(100);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdentityNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LevelName).HasMaxLength(50);
            entity.Property(e => e.Nationality).HasMaxLength(100);
            entity.Property(e => e.NationalityId).HasColumnName("NationalityID");
            entity.Property(e => e.PassportNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PersonId).HasColumnName("PersonID");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(13)
                .IsUnicode(false);
            entity.Property(e => e.RelationshipName).HasMaxLength(50);
            entity.Property(e => e.RelationshipTypeId).HasColumnName("RelationshipTypeID");
            entity.Property(e => e.SecondName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ThirdName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UniversityId).HasColumnName("UniversityID");
            entity.Property(e => e.UniversityName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("university_name");
        });

        modelBuilder.Entity<WorkingDay>(entity =>
        {
            entity.HasKey(e => e.WorkingDaysId).HasName("PK__WorkingD__191253CE5C71E0D9");

            entity.HasOne(d => d.Employee).WithMany(p => p.WorkingDays)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__WorkingDa__Emplo__2D7CBDC4");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
