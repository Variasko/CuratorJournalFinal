USE [master]
GO
/****** Object:  Database [CuratorJournal]    Script Date: 20.06.2025 1:34:07 ******/
CREATE DATABASE [CuratorJournal]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'CuratorJournal', FILENAME = N'C:\Program Files\Microsoft SQL Server1\MSSQL15.SQLEXPRESS01\MSSQL\DATA\CuratorJournal.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'CuratorJournal_log', FILENAME = N'C:\Program Files\Microsoft SQL Server1\MSSQL15.SQLEXPRESS01\MSSQL\DATA\CuratorJournal_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [CuratorJournal] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CuratorJournal].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [CuratorJournal] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [CuratorJournal] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [CuratorJournal] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [CuratorJournal] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [CuratorJournal] SET ARITHABORT OFF 
GO
ALTER DATABASE [CuratorJournal] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [CuratorJournal] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [CuratorJournal] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [CuratorJournal] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [CuratorJournal] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [CuratorJournal] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [CuratorJournal] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [CuratorJournal] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [CuratorJournal] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [CuratorJournal] SET  ENABLE_BROKER 
GO
ALTER DATABASE [CuratorJournal] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [CuratorJournal] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [CuratorJournal] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [CuratorJournal] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [CuratorJournal] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [CuratorJournal] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [CuratorJournal] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [CuratorJournal] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [CuratorJournal] SET  MULTI_USER 
GO
ALTER DATABASE [CuratorJournal] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [CuratorJournal] SET DB_CHAINING OFF 
GO
ALTER DATABASE [CuratorJournal] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [CuratorJournal] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [CuratorJournal] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [CuratorJournal] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [CuratorJournal] SET QUERY_STORE = OFF
GO
USE [CuratorJournal]
GO
/****** Object:  Table [dbo].[ClassHour]    Script Date: 20.06.2025 1:34:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ClassHour](
	[ClassHourId] [int] IDENTITY(1,1) NOT NULL,
	[Date] [date] NOT NULL,
	[Topic] [text] NOT NULL,
	[Decision] [text] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ClassHourId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Curator]    Script Date: 20.06.2025 1:34:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Curator](
	[CuratorId] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[CategoryId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CuratorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CuratorCharacteristic]    Script Date: 20.06.2025 1:34:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CuratorCharacteristic](
	[CharacteristicId] [int] IDENTITY(1,1) NOT NULL,
	[StudentId] [int] NOT NULL,
	[CuratorId] [int] NOT NULL,
	[Characteristic] [text] NOT NULL,
	[DateCreated] [date] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CharacteristicId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GroupPost]    Script Date: 20.06.2025 1:34:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GroupPost](
	[PostId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK__GroupPos__AA126018E843644C] PRIMARY KEY CLUSTERED 
(
	[PostId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Hobby]    Script Date: 20.06.2025 1:34:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Hobby](
	[HobbyId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](30) NOT NULL,
 CONSTRAINT [PK__Hobby__0ABE0BCF15601C82] PRIMARY KEY CLUSTERED 
(
	[HobbyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IndividualWork]    Script Date: 20.06.2025 1:34:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IndividualWork](
	[IndividualWorkId] [int] IDENTITY(1,1) NOT NULL,
	[IsStudent] [bit] NOT NULL,
	[StudentId] [int] NULL,
	[ParentId] [int] NULL,
	[Topic] [text] NOT NULL,
	[Decision] [text] NOT NULL,
	[Date] [date] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IndividualWorkId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Parent]    Script Date: 20.06.2025 1:34:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Parent](
	[ParentId] [int] IDENTITY(1,1) NOT NULL,
	[Surname] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Patronymic] [nvarchar](50) NULL,
	[Phone] [nvarchar](12) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ParentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ParentMeeting]    Script Date: 20.06.2025 1:34:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParentMeeting](
	[MeetingId] [int] IDENTITY(1,1) NOT NULL,
	[StudyGroupId] [int] NOT NULL,
	[Visited] [int] NOT NULL,
	[NotVisitedWithReason] [int] NOT NULL,
	[NotVisited] [int] NOT NULL,
	[Topic] [text] NOT NULL,
	[Decision] [text] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MeetingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Person]    Script Date: 20.06.2025 1:34:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Person](
	[PersonId] [int] IDENTITY(1,1) NOT NULL,
	[Surname] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Patronymic] [nvarchar](50) NULL,
	[PassportSerial] [nvarchar](4) NOT NULL,
	[PassportNumber] [nvarchar](6) NOT NULL,
	[WhoGavePassport] [nvarchar](50) NOT NULL,
	[WhenGetPassport] [date] NOT NULL,
	[INN] [nvarchar](12) NOT NULL,
	[SNILS] [nvarchar](11) NOT NULL,
	[Photo] [varbinary](max) NULL,
	[Phone] [nvarchar](12) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Qualification]    Script Date: 20.06.2025 1:34:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Qualification](
	[Abbreviation] [nvarchar](3) NOT NULL,
	[Name] [nvarchar](60) NOT NULL,
 CONSTRAINT [PK__Speciali__9C41170F89866CA3] PRIMARY KEY CLUSTERED 
(
	[Abbreviation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SocialStatus]    Script Date: 20.06.2025 1:34:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SocialStatus](
	[SocialStatusId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK__SocialSt__B2D12E6B5E39FAF9] PRIMARY KEY CLUSTERED 
(
	[SocialStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Specification]    Script Date: 20.06.2025 1:34:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Specification](
	[Abbreviation] [nvarchar](3) NOT NULL,
	[Name] [nvarchar](60) NOT NULL,
 CONSTRAINT [PK__Directio__9C41170FDB2E6D3D] PRIMARY KEY CLUSTERED 
(
	[Abbreviation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Student]    Script Date: 20.06.2025 1:34:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Student](
	[StudentId] [int] NOT NULL,
	[IsDeduction] [bit] NOT NULL,
	[DateDeduction] [date] NULL,
 CONSTRAINT [PK__Student__32C52B9950AF0A31] PRIMARY KEY CLUSTERED 
(
	[StudentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StudentClassHour]    Script Date: 20.06.2025 1:34:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StudentClassHour](
	[ClassHourId] [int] NOT NULL,
	[StudentId] [int] NOT NULL,
	[IsVisited] [bit] NOT NULL,
 CONSTRAINT [PK_StudentClassHour] PRIMARY KEY CLUSTERED 
(
	[ClassHourId] ASC,
	[StudentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StudentHobby]    Script Date: 20.06.2025 1:34:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StudentHobby](
	[StudentId] [int] NOT NULL,
	[HobbyId] [int] NOT NULL,
 CONSTRAINT [PK_StudentHobby] PRIMARY KEY CLUSTERED 
(
	[StudentId] ASC,
	[HobbyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StudentInDormitory]    Script Date: 20.06.2025 1:34:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StudentInDormitory](
	[StudentId] [int] NOT NULL,
	[Room] [int] NOT NULL,
 CONSTRAINT [PK_StudentInDormitory_1] PRIMARY KEY CLUSTERED 
(
	[StudentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StudentParent]    Script Date: 20.06.2025 1:34:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StudentParent](
	[ParentId] [int] NOT NULL,
	[StudentId] [int] NOT NULL,
 CONSTRAINT [PK_StudentParent] PRIMARY KEY CLUSTERED 
(
	[ParentId] ASC,
	[StudentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StudentSocialStatus]    Script Date: 20.06.2025 1:34:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StudentSocialStatus](
	[SocialStatusId] [int] NOT NULL,
	[StudentId] [int] NOT NULL,
 CONSTRAINT [PK_StudentSocialStatus] PRIMARY KEY CLUSTERED 
(
	[SocialStatusId] ASC,
	[StudentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StudentStudyGroup]    Script Date: 20.06.2025 1:34:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StudentStudyGroup](
	[StudentId] [int] NOT NULL,
	[StudyGroupId] [int] NOT NULL,
 CONSTRAINT [PK_StudentStudyGroup] PRIMARY KEY CLUSTERED 
(
	[StudentId] ASC,
	[StudyGroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StudyGroup]    Script Date: 20.06.2025 1:34:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StudyGroup](
	[StudyGroupId] [int] IDENTITY(1,1) NOT NULL,
	[SpecificationAbbreviation] [nvarchar](3) NOT NULL,
	[QualificationAbbreviation] [nvarchar](3) NULL,
	[Course] [int] NOT NULL,
	[DateCreate] [date] NOT NULL,
	[IsBuget] [bit] NOT NULL,
	[FullName]  AS ((((([SpecificationAbbreviation]+'-')+CONVERT([char](1),[Course]))+right(CONVERT([varchar](4),datepart(year,[DateCreate])),(2)))+isnull([QualificationAbbreviation],''))+case when [IsBuget]=(0) then 'в' else '' end),
	[CuratorId] [int] NOT NULL,
 CONSTRAINT [PK__StudyGro__D97A1BE27006F813] PRIMARY KEY CLUSTERED 
(
	[StudyGroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StudyGroupPost]    Script Date: 20.06.2025 1:34:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StudyGroupPost](
	[StudentId] [int] NOT NULL,
	[PostId] [int] NOT NULL,
 CONSTRAINT [PK_StudyGroupPost] PRIMARY KEY CLUSTERED 
(
	[StudentId] ASC,
	[PostId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TeacherCategory]    Script Date: 20.06.2025 1:34:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TeacherCategory](
	[CategoryId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](33) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 20.06.2025 1:34:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[Login] [nvarchar](30) NOT NULL,
	[Password] [nvarchar](30) NOT NULL,
	[IsAdmin] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Curator] ON 

INSERT [dbo].[Curator] ([CuratorId], [PersonId], [CategoryId], [UserId]) VALUES (1, 1, 1, 1)
SET IDENTITY_INSERT [dbo].[Curator] OFF
GO
SET IDENTITY_INSERT [dbo].[Person] ON 

INSERT [dbo].[Person] ([PersonId], [Surname], [Name], [Patronymic], [PassportSerial], [PassportNumber], [WhoGavePassport], [WhenGetPassport], [INN], [SNILS], [Photo], [Phone], [Email]) VALUES (1, N'MASTER', N'MASTER', N'MASTER', N'0', N'0', N'0', CAST(N'2001-01-01' AS Date), N'0', N'0', NULL, N'0', N'0')
SET IDENTITY_INSERT [dbo].[Person] OFF
GO
SET IDENTITY_INSERT [dbo].[TeacherCategory] ON 

INSERT [dbo].[TeacherCategory] ([CategoryId], [Name]) VALUES (1, N'MASTER')
SET IDENTITY_INSERT [dbo].[TeacherCategory] OFF
GO
SET IDENTITY_INSERT [dbo].[User] ON 

INSERT [dbo].[User] ([UserId], [Login], [Password], [IsAdmin]) VALUES (1, N'MASTER', N'MASTER', 1)
SET IDENTITY_INSERT [dbo].[User] OFF
GO
ALTER TABLE [dbo].[CuratorCharacteristic] ADD  DEFAULT (getdate()) FOR [DateCreated]
GO
ALTER TABLE [dbo].[IndividualWork] ADD  DEFAULT ((1)) FOR [IsStudent]
GO
ALTER TABLE [dbo].[StudentClassHour] ADD  DEFAULT ((0)) FOR [IsVisited]
GO
ALTER TABLE [dbo].[StudyGroup] ADD  CONSTRAINT [DF__StudyGrou__IsBug__38996AB5]  DEFAULT ((1)) FOR [IsBuget]
GO
ALTER TABLE [dbo].[User] ADD  DEFAULT ((0)) FOR [IsAdmin]
GO
ALTER TABLE [dbo].[Curator]  WITH CHECK ADD FOREIGN KEY([CategoryId])
REFERENCES [dbo].[TeacherCategory] ([CategoryId])
GO
ALTER TABLE [dbo].[Curator]  WITH CHECK ADD FOREIGN KEY([PersonId])
REFERENCES [dbo].[Person] ([PersonId])
GO
ALTER TABLE [dbo].[Curator]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[CuratorCharacteristic]  WITH CHECK ADD FOREIGN KEY([CuratorId])
REFERENCES [dbo].[Curator] ([CuratorId])
GO
ALTER TABLE [dbo].[CuratorCharacteristic]  WITH CHECK ADD  CONSTRAINT [FK__CuratorCh__Stude__59063A47] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Student] ([StudentId])
GO
ALTER TABLE [dbo].[CuratorCharacteristic] CHECK CONSTRAINT [FK__CuratorCh__Stude__59063A47]
GO
ALTER TABLE [dbo].[IndividualWork]  WITH CHECK ADD FOREIGN KEY([ParentId])
REFERENCES [dbo].[Parent] ([ParentId])
GO
ALTER TABLE [dbo].[IndividualWork]  WITH CHECK ADD  CONSTRAINT [FK__Individua__Stude__5BE2A6F2] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Student] ([StudentId])
GO
ALTER TABLE [dbo].[IndividualWork] CHECK CONSTRAINT [FK__Individua__Stude__5BE2A6F2]
GO
ALTER TABLE [dbo].[ParentMeeting]  WITH CHECK ADD  CONSTRAINT [FK__ParentMee__Study__5DCAEF64] FOREIGN KEY([StudyGroupId])
REFERENCES [dbo].[StudyGroup] ([StudyGroupId])
GO
ALTER TABLE [dbo].[ParentMeeting] CHECK CONSTRAINT [FK__ParentMee__Study__5DCAEF64]
GO
ALTER TABLE [dbo].[Student]  WITH CHECK ADD  CONSTRAINT [FK__Student__Student__5EBF139D] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Person] ([PersonId])
GO
ALTER TABLE [dbo].[Student] CHECK CONSTRAINT [FK__Student__Student__5EBF139D]
GO
ALTER TABLE [dbo].[StudentClassHour]  WITH CHECK ADD FOREIGN KEY([ClassHourId])
REFERENCES [dbo].[ClassHour] ([ClassHourId])
GO
ALTER TABLE [dbo].[StudentClassHour]  WITH CHECK ADD  CONSTRAINT [FK__StudentCl__Stude__60A75C0F] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Student] ([StudentId])
GO
ALTER TABLE [dbo].[StudentClassHour] CHECK CONSTRAINT [FK__StudentCl__Stude__60A75C0F]
GO
ALTER TABLE [dbo].[StudentHobby]  WITH CHECK ADD FOREIGN KEY([HobbyId])
REFERENCES [dbo].[Hobby] ([HobbyId])
GO
ALTER TABLE [dbo].[StudentHobby]  WITH CHECK ADD  CONSTRAINT [FK__StudentHo__Stude__619B8048] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Student] ([StudentId])
GO
ALTER TABLE [dbo].[StudentHobby] CHECK CONSTRAINT [FK__StudentHo__Stude__619B8048]
GO
ALTER TABLE [dbo].[StudentInDormitory]  WITH CHECK ADD  CONSTRAINT [FK__StudentIn__Stude__6383C8BA] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Student] ([StudentId])
GO
ALTER TABLE [dbo].[StudentInDormitory] CHECK CONSTRAINT [FK__StudentIn__Stude__6383C8BA]
GO
ALTER TABLE [dbo].[StudentParent]  WITH CHECK ADD FOREIGN KEY([ParentId])
REFERENCES [dbo].[Parent] ([ParentId])
GO
ALTER TABLE [dbo].[StudentParent]  WITH CHECK ADD  CONSTRAINT [FK__StudentPa__Stude__656C112C] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Student] ([StudentId])
GO
ALTER TABLE [dbo].[StudentParent] CHECK CONSTRAINT [FK__StudentPa__Stude__656C112C]
GO
ALTER TABLE [dbo].[StudentSocialStatus]  WITH CHECK ADD FOREIGN KEY([SocialStatusId])
REFERENCES [dbo].[SocialStatus] ([SocialStatusId])
GO
ALTER TABLE [dbo].[StudentSocialStatus]  WITH CHECK ADD  CONSTRAINT [FK__StudentSo__Stude__66603565] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Student] ([StudentId])
GO
ALTER TABLE [dbo].[StudentSocialStatus] CHECK CONSTRAINT [FK__StudentSo__Stude__66603565]
GO
ALTER TABLE [dbo].[StudentStudyGroup]  WITH CHECK ADD  CONSTRAINT [FK__StudentSt__Stude__68487DD7] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Student] ([StudentId])
GO
ALTER TABLE [dbo].[StudentStudyGroup] CHECK CONSTRAINT [FK__StudentSt__Stude__68487DD7]
GO
ALTER TABLE [dbo].[StudentStudyGroup]  WITH CHECK ADD  CONSTRAINT [FK__StudentSt__Study__693CA210] FOREIGN KEY([StudyGroupId])
REFERENCES [dbo].[StudyGroup] ([StudyGroupId])
GO
ALTER TABLE [dbo].[StudentStudyGroup] CHECK CONSTRAINT [FK__StudentSt__Study__693CA210]
GO
ALTER TABLE [dbo].[StudyGroup]  WITH CHECK ADD  CONSTRAINT [FK_StudyGroup_Curator] FOREIGN KEY([CuratorId])
REFERENCES [dbo].[Curator] ([CuratorId])
GO
ALTER TABLE [dbo].[StudyGroup] CHECK CONSTRAINT [FK_StudyGroup_Curator]
GO
ALTER TABLE [dbo].[StudyGroup]  WITH CHECK ADD  CONSTRAINT [FK_StudyGroup_Qualification] FOREIGN KEY([QualificationAbbreviation])
REFERENCES [dbo].[Qualification] ([Abbreviation])
GO
ALTER TABLE [dbo].[StudyGroup] CHECK CONSTRAINT [FK_StudyGroup_Qualification]
GO
ALTER TABLE [dbo].[StudyGroup]  WITH CHECK ADD  CONSTRAINT [FK_StudyGroup_Specification] FOREIGN KEY([SpecificationAbbreviation])
REFERENCES [dbo].[Specification] ([Abbreviation])
GO
ALTER TABLE [dbo].[StudyGroup] CHECK CONSTRAINT [FK_StudyGroup_Specification]
GO
ALTER TABLE [dbo].[StudyGroupPost]  WITH CHECK ADD FOREIGN KEY([PostId])
REFERENCES [dbo].[GroupPost] ([PostId])
GO
ALTER TABLE [dbo].[StudyGroupPost]  WITH CHECK ADD  CONSTRAINT [FK__StudyGrou__Stude__6C190EBB] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Student] ([StudentId])
GO
ALTER TABLE [dbo].[StudyGroupPost] CHECK CONSTRAINT [FK__StudyGrou__Stude__6C190EBB]
GO
ALTER TABLE [dbo].[StudyGroup]  WITH CHECK ADD  CONSTRAINT [CK__StudyGrou__Cours__NewRange] CHECK  (([Course]>=(0) AND [Course]<=(4)))
GO
ALTER TABLE [dbo].[StudyGroup] CHECK CONSTRAINT [CK__StudyGrou__Cours__NewRange]
GO
USE [master]
GO
ALTER DATABASE [CuratorJournal] SET  READ_WRITE 
GO
