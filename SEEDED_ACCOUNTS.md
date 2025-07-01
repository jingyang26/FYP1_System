# FYP1 System - Seeded Accounts

This document lists all the seeded accounts created for testing the FYP1 Management System.

**Note**: This seeding system combines and enhances the functionality from the original SeedData class to provide comprehensive test data including multiple user accounts, realistic proposals, comments, and evaluator assignments.

## Features
- **Robust error handling**: Gracefully handles missing programs
- **Multiple accounts per role**: Comprehensive coverage across all user types
- **Realistic test data**: Malaysian names, proper student IDs, GPAs, and academic data
- **Sample proposals**: Automatically created for students with supervisors
- **Comment system**: Sample feedback conversations between supervisors and students
- **Evaluator assignments**: Pre-configured evaluation assignments for testing
- **Cross-program coverage**: Supports 4 academic programs

## Default Password
All accounts use the following password pattern:
- **Admin accounts**: `Admin@123`
- **Committee accounts**: `Committee@123`
- **Supervisor accounts**: `Supervisor@123`
- **Evaluator accounts**: `Evaluator@123`
- **Student accounts**: `Student@123`

## Admin Accounts (2 accounts)

| Email | Full Name | Role |
|-------|-----------|------|
| admin@fyp1.edu.my | System Administrator | Admin |
| admin2@fyp1.edu.my | Deputy Administrator | Admin |

## Committee Accounts (6 accounts)

| Email | Full Name | Program | Domain | Office | Role |
|-------|-----------|---------|--------|--------|------|
| committee.se@fyp1.edu.my | Dr. John Smith | Software Engineering | Software Engineering | Block A, Level 2 | Chair |
| committee.de@fyp1.edu.my | Dr. Lisa Wong | Data Engineering | Data Engineering | Block C, Level 2 | Chair |
| committee.cs@fyp1.edu.my | Prof. David Lee | Computer Science | Computer Science | Block B, Level 3 | Chair |
| committee.it@fyp1.edu.my | Dr. Maria Garcia | Information Technology | Information Technology | Block D, Level 1 | Chair |
| committee2.se@fyp1.edu.my | Dr. Ahmed Hassan | Software Engineering | Software Engineering | Block A, Level 3 | Member |
| committee2.de@fyp1.edu.my | Dr. Jennifer Chen | Data Engineering | Data Engineering | Block C, Level 3 | Member |

## Supervisor Accounts (9 accounts)

| Email | Full Name | Program | Domain | Office |
|-------|-----------|---------|--------|--------|
| supervisor1.se@fyp1.edu.my | Dr. Sarah Johnson | Software Engineering | Web Development | Block A, Level 4 |
| supervisor2.se@fyp1.edu.my | Dr. Michael Brown | Software Engineering | Mobile Development | Block A, Level 5 |
| supervisor3.se@fyp1.edu.my | Dr. Emily Davis | Software Engineering | Software Testing | Block A, Level 6 |
| supervisor1.de@fyp1.edu.my | Prof. Robert Wilson | Data Engineering | Machine Learning | Block C, Level 4 |
| supervisor2.de@fyp1.edu.my | Dr. Linda Taylor | Data Engineering | Big Data Analytics | Block C, Level 5 |
| supervisor1.cs@fyp1.edu.my | Dr. James Anderson | Computer Science | Artificial Intelligence | Block B, Level 4 |
| supervisor2.cs@fyp1.edu.my | Dr. Patricia Moore | Computer Science | Computer Graphics | Block B, Level 5 |
| supervisor1.it@fyp1.edu.my | Dr. Christopher Jackson | Information Technology | Network Security | Block D, Level 2 |
| supervisor2.it@fyp1.edu.my | Dr. Barbara White | Information Technology | Database Systems | Block D, Level 3 |

## Evaluator Accounts (8 accounts)

| Email | Full Name | Program | Domain | Office |
|-------|-----------|---------|--------|--------|
| evaluator1.se@fyp1.edu.my | Prof. Kevin Martinez | Software Engineering | Software Architecture | Block A, Level 7 |
| evaluator2.se@fyp1.edu.my | Dr. Nancy Rodriguez | Software Engineering | System Analysis | Block A, Level 8 |
| evaluator1.de@fyp1.edu.my | Dr. Steven Lewis | Data Engineering | Data Mining | Block C, Level 6 |
| evaluator2.de@fyp1.edu.my | Dr. Dorothy Clark | Data Engineering | Statistical Analysis | Block C, Level 7 |
| evaluator1.cs@fyp1.edu.my | Prof. Mark Robinson | Computer Science | Algorithm Design | Block B, Level 6 |
| evaluator2.cs@fyp1.edu.my | Dr. Helen Walker | Computer Science | Computational Theory | Block B, Level 7 |
| evaluator1.it@fyp1.edu.my | Dr. Paul Hall | Information Technology | IT Infrastructure | Block D, Level 4 |
| evaluator2.it@fyp1.edu.my | Dr. Sandra Allen | Information Technology | Information Systems | Block D, Level 5 |

## Student Accounts (16 accounts)

### Software Engineering Students (5 students)
| Email | Full Name | Student ID | GPA | Session | Semester |
|-------|-----------|------------|-----|---------|----------|
| student1.se@fyp1.edu.my | Ahmad Ali | SE12001 | 3.75 | 2024/2025 | 1 |
| student2.se@fyp1.edu.my | Siti Nurhaliza | SE12002 | 3.85 | 2024/2025 | 1 |
| student3.se@fyp1.edu.my | Lim Wei Ming | SE12003 | 3.65 | 2024/2025 | 1 |
| student4.se@fyp1.edu.my | Raj Kumar | SE12004 | 3.90 | 2024/2025 | 1 |
| student5.se@fyp1.edu.my | Fatimah Zahra | SE12005 | 3.55 | 2024/2025 | 1 |

### Data Engineering Students (4 students)
| Email | Full Name | Student ID | GPA | Session | Semester |
|-------|-----------|------------|-----|---------|----------|
| student1.de@fyp1.edu.my | Chen Wei Jie | DE12001 | 3.80 | 2024/2025 | 1 |
| student2.de@fyp1.edu.my | Nurul Ain | DE12002 | 3.70 | 2024/2025 | 1 |
| student3.de@fyp1.edu.my | Krishna Mohan | DE12003 | 3.95 | 2024/2025 | 1 |
| student4.de@fyp1.edu.my | Wong Kok Wai | DE12004 | 3.60 | 2024/2025 | 1 |

### Computer Science Students (3 students)
| Email | Full Name | Student ID | GPA | Session | Semester |
|-------|-----------|------------|-----|---------|----------|
| student1.cs@fyp1.edu.my | Muhammad Hafiz | CS12001 | 3.78 | 2024/2025 | 1 |
| student2.cs@fyp1.edu.my | Lee Mei Ling | CS12002 | 3.88 | 2024/2025 | 1 |
| student3.cs@fyp1.edu.my | Arjun Patel | CS12003 | 3.72 | 2024/2025 | 1 |

### Information Technology Students (3 students)
| Email | Full Name | Student ID | GPA | Session | Semester |
|-------|-----------|------------|-----|---------|----------|
| student1.it@fyp1.edu.my | Zainab Ibrahim | IT12001 | 3.82 | 2024/2025 | 1 |
| student2.it@fyp1.edu.my | Tan Boon Hock | IT12002 | 3.67 | 2024/2025 | 1 |
| student3.it@fyp1.edu.my | Priya Sharma | IT12003 | 3.93 | 2024/2025 | 1 |

## Programs Available

1. **Software Engineering** - Bachelor of Software Engineering program
2. **Data Engineering** - Bachelor of Data Engineering program
3. **Computer Science** - Bachelor of Computer Science program
4. **Information Technology** - Bachelor of Information Technology program

## Notes

- Some students have been automatically assigned supervisors during the seeding process
- All accounts have EmailConfirmed set to true for easy testing
- Lecturers have appropriate domain specializations matching their programs
- Committee members have proper roles (Chair/Member) assigned
- All users are assigned to appropriate programs based on their roles

## Testing Instructions

1. Run the application
2. The seeding will happen automatically on first run
3. Use any of the accounts above to log in with their respective passwords
4. Test role-based functionality with different account types

## Security Note

**Important**: These are test accounts with simple passwords. In production:
- Use complex, unique passwords
- Implement proper password policies
- Consider multi-factor authentication
- Remove or disable test accounts
