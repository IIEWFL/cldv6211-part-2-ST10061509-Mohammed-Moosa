-- Ensure correct schema usage
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Venues]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.Venues (
        VenueId INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(255) NOT NULL,
        Location NVARCHAR(255) NOT NULL,
        Capacity INT NOT NULL,
        ImageUrl NVARCHAR(500) DEFAULT 'https://via.placeholder.com/150',
        IsAvailable BIT DEFAULT 1
    );
END
GO

-- Create the Events Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Events]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.Events (
        EventId INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(255) NOT NULL,
        Description NVARCHAR(MAX),
        StartDate DATETIME NOT NULL,
        EndDate DATETIME NOT NULL,
        VenueId INT NULL,
        CONSTRAINT FK_Events_Venues FOREIGN KEY (VenueId) REFERENCES dbo.Venues(VenueId) ON DELETE SET NULL
    );
END
GO

-- Create the Bookings Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Bookings]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.Bookings (
        BookingId INT IDENTITY(1,1) PRIMARY KEY,
        CustomerName NVARCHAR(255) NOT NULL,
        ContactEmail NVARCHAR(255) NOT NULL,
        VenueId INT  NOT NULL,
        EventId INT NOT NULL,
        BookingDate DATETIME DEFAULT GETDATE(),
        CONSTRAINT FK_Bookings_Venues FOREIGN KEY (VenueId) REFERENCES dbo.Venues(VenueId),
        CONSTRAINT FK_Bookings_Events FOREIGN KEY (EventId) REFERENCES dbo.Events(EventId)
    );
END
GO

-- Insert Sample Data into Venues
IF NOT EXISTS (SELECT 1 FROM dbo.Venues)
BEGIN
    INSERT INTO dbo.Venues (Name, Location, Capacity)
    VALUES 
    ('Grand Hall', 'Johannesburg', 500),
    ('Conference Room A', 'Cape Town', 200),
    ('Outdoor Garden', 'Durban', 1000);
END
GO

-- Insert Sample Data into Events
IF NOT EXISTS (SELECT 1 FROM dbo.Events)
BEGIN
    INSERT INTO dbo.Events (Name, Description, StartDate, EndDate, VenueId)
    VALUES 
    ('Tech Summit 2025', 'Annual tech conference', '2025-06-15', '2025-06-16', 1),
    ('Business Networking', 'Professional networking event', '2025-07-01', '2025-07-01', 2),
    ('Music Festival', 'Live performances by various artists', '2025-08-10', '2025-08-12', 3);
END
GO

-- Insert Sample Data into Bookings
IF NOT EXISTS (SELECT 1 FROM dbo.Bookings)
BEGIN
    INSERT INTO dbo.Bookings (CustomerName, ContactEmail, VenueId, EventId)
    VALUES 
    ('John Doe', 'john@example.com', 1, 1),
    ('Alice Smith', 'alice@example.com', 2, 2),
    ('Mark Johnson', 'mark@example.com', 3, 3);
END
GO

-- Create the BookingEnhanced View (Only Name, EventId, BookingDate)
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[BookingEnhanced]'))
BEGIN
    EXEC('CREATE VIEW dbo.BookingEnhanced AS 
        SELECT 
            b.CustomerName AS Name, 
            b.EventId, 
            b.BookingDate
        FROM dbo.Bookings b');
END
GO

-- Verify Data
SELECT * FROM dbo.Venues;
SELECT * FROM dbo.Events;
SELECT * FROM dbo.Bookings;
SELECT * FROM dbo.BookingEnhanced;
GO
