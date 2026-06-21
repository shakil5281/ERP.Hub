using System.Collections.Generic;
using System.Linq;

namespace ERPHub.Models;

public static class BangladeshAddressData
{
    public static List<AddressDivision> Divisions => new()
        {
            new() { Id = 1, Name = "Barishal" },
            new() { Id = 2, Name = "Chattogram" },
            new() { Id = 3, Name = "Dhaka" },
            new() { Id = 4, Name = "Khulna" },
            new() { Id = 5, Name = "Mymensingh" },
            new() { Id = 6, Name = "Rajshahi" },
            new() { Id = 7, Name = "Rangpur" },
            new() { Id = 8, Name = "Sylhet" }
        };

    public static List<AddressDistrict> Districts => new()
        {
            // Barishal Division
            new() { Id = 1, Name = "Barguna", DivisionId = 1 },
            new() { Id = 2, Name = "Barishal", DivisionId = 1 },
            new() { Id = 3, Name = "Bhola", DivisionId = 1 },
            new() { Id = 4, Name = "Jhalakathi", DivisionId = 1 },
            new() { Id = 5, Name = "Patuakhali", DivisionId = 1 },
            new() { Id = 6, Name = "Pirojpur", DivisionId = 1 },

            // Chattogram Division
            new() { Id = 7, Name = "Bandarban", DivisionId = 2 },
            new() { Id = 8, Name = "Brahmanbaria", DivisionId = 2 },
            new() { Id = 9, Name = "Chandpur", DivisionId = 2 },
            new() { Id = 10, Name = "Chattogram", DivisionId = 2 },
            new() { Id = 11, Name = "Comilla", DivisionId = 2 },
            new() { Id = 12, Name = "Cox's Bazar", DivisionId = 2 },
            new() { Id = 13, Name = "Feni", DivisionId = 2 },
            new() { Id = 14, Name = "Khagrachhari", DivisionId = 2 },
            new() { Id = 15, Name = "Lakshmipur", DivisionId = 2 },
            new() { Id = 16, Name = "Noakhali", DivisionId = 2 },
            new() { Id = 17, Name = "Rangamati", DivisionId = 2 },

            // Dhaka Division
            new() { Id = 18, Name = "Dhaka", DivisionId = 3 },
            new() { Id = 19, Name = "Faridpur", DivisionId = 3 },
            new() { Id = 20, Name = "Gazipur", DivisionId = 3 },
            new() { Id = 21, Name = "Gopalganj", DivisionId = 3 },
            new() { Id = 22, Name = "Kishoreganj", DivisionId = 3 },
            new() { Id = 23, Name = "Madaripur", DivisionId = 3 },
            new() { Id = 24, Name = "Manikganj", DivisionId = 3 },
            new() { Id = 25, Name = "Munshiganj", DivisionId = 3 },
            new() { Id = 26, Name = "Narayanganj", DivisionId = 3 },
            new() { Id = 27, Name = "Narsingdi", DivisionId = 3 },
            new() { Id = 28, Name = "Rajbari", DivisionId = 3 },
            new() { Id = 29, Name = "Shariatpur", DivisionId = 3 },
            new() { Id = 30, Name = "Tangail", DivisionId = 3 },

            // Khulna Division
            new() { Id = 31, Name = "Bagerhat", DivisionId = 4 },
            new() { Id = 32, Name = "Chuadanga", DivisionId = 4 },
            new() { Id = 33, Name = "Jashore", DivisionId = 4 },
            new() { Id = 34, Name = "Jhenaidah", DivisionId = 4 },
            new() { Id = 35, Name = "Khulna", DivisionId = 4 },
            new() { Id = 36, Name = "Kushtia", DivisionId = 4 },
            new() { Id = 37, Name = "Magura", DivisionId = 4 },
            new() { Id = 38, Name = "Meherpur", DivisionId = 4 },
            new() { Id = 39, Name = "Narail", DivisionId = 4 },
            new() { Id = 40, Name = "Satkhira", DivisionId = 4 },

            // Mymensingh Division
            new() { Id = 41, Name = "Jamalpur", DivisionId = 5 },
            new() { Id = 42, Name = "Mymensingh", DivisionId = 5 },
            new() { Id = 43, Name = "Netrokona", DivisionId = 5 },
            new() { Id = 44, Name = "Sherpur", DivisionId = 5 },

            // Rajshahi Division
            new() { Id = 45, Name = "Bogura", DivisionId = 6 },
            new() { Id = 46, Name = "Joypurhat", DivisionId = 6 },
            new() { Id = 47, Name = "Naogaon", DivisionId = 6 },
            new() { Id = 48, Name = "Natore", DivisionId = 6 },
            new() { Id = 49, Name = "Chapainawabganj", DivisionId = 6 },
            new() { Id = 50, Name = "Pabna", DivisionId = 6 },
            new() { Id = 51, Name = "Rajshahi", DivisionId = 6 },
            new() { Id = 52, Name = "Sirajganj", DivisionId = 6 },

            // Rangpur Division
            new() { Id = 53, Name = "Dinajpur", DivisionId = 7 },
            new() { Id = 54, Name = "Gaibandha", DivisionId = 7 },
            new() { Id = 55, Name = "Kurigram", DivisionId = 7 },
            new() { Id = 56, Name = "Lalmonirhat", DivisionId = 7 },
            new() { Id = 57, Name = "Nilphamari", DivisionId = 7 },
            new() { Id = 58, Name = "Panchagarh", DivisionId = 7 },
            new() { Id = 59, Name = "Rangpur", DivisionId = 7 },
            new() { Id = 60, Name = "Thakurgaon", DivisionId = 7 },

            // Sylhet Division
            new() { Id = 61, Name = "Habiganj", DivisionId = 8 },
            new() { Id = 62, Name = "Moulvibazar", DivisionId = 8 },
            new() { Id = 63, Name = "Sunamganj", DivisionId = 8 },
            new() { Id = 64, Name = "Sylhet", DivisionId = 8 }
        };

    public static List<AddressUpazila> Upazilas => new()
        {
            // Barguna
            new() { Id = 1, Name = "Amtali", DistrictId = 1 },
            new() { Id = 2, Name = "Barguna Sadar", DistrictId = 1 },
            new() { Id = 3, Name = "Betagi", DistrictId = 1 },
            new() { Id = 4, Name = "Bamna", DistrictId = 1 },
            new() { Id = 5, Name = "Patharghata", DistrictId = 1 },
            new() { Id = 6, Name = "Taltali", DistrictId = 1 },

            // Barishal
            new() { Id = 7, Name = "Agailjhara", DistrictId = 2 },
            new() { Id = 8, Name = "Babuganj", DistrictId = 2 },
            new() { Id = 9, Name = "Bakerganj", DistrictId = 2 },
            new() { Id = 10, Name = "Banari Para", DistrictId = 2 },
            new() { Id = 11, Name = "Barishal Sadar", DistrictId = 2 },
            new() { Id = 12, Name = "Gournadi", DistrictId = 2 },
            new() { Id = 13, Name = "Hizla", DistrictId = 2 },
            new() { Id = 14, Name = "Mehendiganj", DistrictId = 2 },
            new() { Id = 15, Name = "Muladi", DistrictId = 2 },
            new() { Id = 16, Name = "Wazirpur", DistrictId = 2 },

            // Bhola
            new() { Id = 17, Name = "Bhola Sadar", DistrictId = 3 },
            new() { Id = 18, Name = "Burhanuddin", DistrictId = 3 },
            new() { Id = 19, Name = "Char Fasson", DistrictId = 3 },
            new() { Id = 20, Name = "Daulatkhan", DistrictId = 3 },
            new() { Id = 21, Name = "Lalmohan", DistrictId = 3 },
            new() { Id = 22, Name = "Manpura", DistrictId = 3 },
            new() { Id = 23, Name = "Tajumuddin", DistrictId = 3 },

            // Jhalakathi
            new() { Id = 24, Name = "Jhalakathi Sadar", DistrictId = 4 },
            new() { Id = 25, Name = "Kathalia", DistrictId = 4 },
            new() { Id = 26, Name = "Nalchity", DistrictId = 4 },
            new() { Id = 27, Name = "Rajapur", DistrictId = 4 },

            // Patuakhali
            new() { Id = 28, Name = "Bauphal", DistrictId = 5 },
            new() { Id = 29, Name = "Dashmina", DistrictId = 5 },
            new() { Id = 30, Name = "Galachipa", DistrictId = 5 },
            new() { Id = 31, Name = "Kalapara", DistrictId = 5 },
            new() { Id = 32, Name = "Mirzaganj", DistrictId = 5 },
            new() { Id = 33, Name = "Patuakhali Sadar", DistrictId = 5 },
            new() { Id = 34, Name = "Rangabali", DistrictId = 5 },

            // Pirojpur
            new() { Id = 35, Name = "Bhandaria", DistrictId = 6 },
            new() { Id = 36, Name = "Kawkhali", DistrictId = 6 },
            new() { Id = 37, Name = "Mathbaria", DistrictId = 6 },
            new() { Id = 38, Name = "Nazirpur", DistrictId = 6 },
            new() { Id = 39, Name = "Nesarabad", DistrictId = 6 },
            new() { Id = 40, Name = "Pirojpur Sadar", DistrictId = 6 },
            new() { Id = 41, Name = "Swarupkathi", DistrictId = 6 },

            // Bandarban
            new() { Id = 42, Name = "Alikadam", DistrictId = 7 },
            new() { Id = 43, Name = "Bandarban Sadar", DistrictId = 7 },
            new() { Id = 44, Name = "Lama", DistrictId = 7 },
            new() { Id = 45, Name = "Naikhongchhari", DistrictId = 7 },
            new() { Id = 46, Name = "Rowangchhari", DistrictId = 7 },
            new() { Id = 47, Name = "Ruma", DistrictId = 7 },
            new() { Id = 48, Name = "Thanchi", DistrictId = 7 },

            // Brahmanbaria
            new() { Id = 49, Name = "Akhaura", DistrictId = 8 },
            new() { Id = 50, Name = "Bancharampur", DistrictId = 8 },
            new() { Id = 51, Name = "Brahmanbaria Sadar", DistrictId = 8 },
            new() { Id = 52, Name = "Ashuganj", DistrictId = 8 },
            new() { Id = 53, Name = "Kasba", DistrictId = 8 },
            new() { Id = 54, Name = "Nabinagar", DistrictId = 8 },
            new() { Id = 55, Name = "Nasirnagar", DistrictId = 8 },
            new() { Id = 56, Name = "Sarail", DistrictId = 8 },

            // Chandpur
            new() { Id = 57, Name = "Chandpur Sadar", DistrictId = 9 },
            new() { Id = 58, Name = "Faridganj", DistrictId = 9 },
            new() { Id = 59, Name = "Haimchar", DistrictId = 9 },
            new() { Id = 60, Name = "Haziganj", DistrictId = 9 },
            new() { Id = 61, Name = "Kachua", DistrictId = 9 },
            new() { Id = 62, Name = "Maherpur", DistrictId = 9 },
            new() { Id = 63, Name = "Shahrasti", DistrictId = 9 },

            // Chattogram
            new() { Id = 64, Name = "Anowara", DistrictId = 10 },
            new() { Id = 65, Name = "Banshkhali", DistrictId = 10 },
            new() { Id = 66, Name = "Boalkhali", DistrictId = 10 },
            new() { Id = 67, Name = "Chandanaish", DistrictId = 10 },
            new() { Id = 68, Name = "Chattogram Sadar", DistrictId = 10 },
            new() { Id = 69, Name = "Fatikchhari", DistrictId = 10 },
            new() { Id = 70, Name = "Hathazari", DistrictId = 10 },
            new() { Id = 71, Name = "Lohagara", DistrictId = 10 },
            new() { Id = 72, Name = "Mirsharai", DistrictId = 10 },
            new() { Id = 73, Name = "Patiya", DistrictId = 10 },
            new() { Id = 74, Name = "Rangunia", DistrictId = 10 },
            new() { Id = 75, Name = "Raozan", DistrictId = 10 },
            new() { Id = 76, Name = "Sandwip", DistrictId = 10 },
            new() { Id = 77, Name = "Satkania", DistrictId = 10 },
            new() { Id = 78, Name = "Sitakunda", DistrictId = 10 },

            // Comilla
            new() { Id = 79, Name = "Barura", DistrictId = 11 },
            new() { Id = 80, Name = "Brahmanpara", DistrictId = 11 },
            new() { Id = 81, Name = "Chandina", DistrictId = 11 },
            new() { Id = 82, Name = "Chauddagram", DistrictId = 11 },
            new() { Id = 83, Name = "Comilla Sadar", DistrictId = 11 },
            new() { Id = 84, Name = "Daudkandi", DistrictId = 11 },
            new() { Id = 85, Name = "Debidwar", DistrictId = 11 },
            new() { Id = 86, Name = "Homna", DistrictId = 11 },
            new() { Id = 87, Name = "Laksam", DistrictId = 11 },
            new() { Id = 88, Name = "Muradnagar", DistrictId = 11 },
            new() { Id = 89, Name = "Nangalkot", DistrictId = 11 },
            new() { Id = 90, Name = "Titas", DistrictId = 11 },

            // Cox's Bazar
            new() { Id = 91, Name = "Chakaria", DistrictId = 12 },
            new() { Id = 92, Name = "Cox's Bazar Sadar", DistrictId = 12 },
            new() { Id = 93, Name = "Kutubdia", DistrictId = 12 },
            new() { Id = 94, Name = "Maheshkhali", DistrictId = 12 },
            new() { Id = 95, Name = "Ramu", DistrictId = 12 },
            new() { Id = 96, Name = "Teknaf", DistrictId = 12 },
            new() { Id = 97, Name = "Ukhia", DistrictId = 12 },

            // Feni
            new() { Id = 98, Name = "Chhagalnaiya", DistrictId = 13 },
            new() { Id = 99, Name = "Daganbhuiyan", DistrictId = 13 },
            new() { Id = 100, Name = "Feni Sadar", DistrictId = 13 },
            new() { Id = 101, Name = "Parshuram", DistrictId = 13 },
            new() { Id = 102, Name = "Sonagazi", DistrictId = 13 },

            // Khagrachhari
            new() { Id = 103, Name = "Dighinala", DistrictId = 14 },
            new() { Id = 104, Name = "Khagrachhari Sadar", DistrictId = 14 },
            new() { Id = 105, Name = "Laxmichhari", DistrictId = 14 },
            new() { Id = 106, Name = "Mahalchhari", DistrictId = 14 },
            new() { Id = 107, Name = "Manikchhari", DistrictId = 14 },
            new() { Id = 108, Name = "Matiranga", DistrictId = 14 },
            new() { Id = 109, Name = "Panchhari", DistrictId = 14 },
            new() { Id = 110, Name = "Ramgarh", DistrictId = 14 },

            // Lakshmipur
            new() { Id = 111, Name = "Kamalnagar", DistrictId = 15 },
            new() { Id = 112, Name = "Lakshmipur Sadar", DistrictId = 15 },
            new() { Id = 113, Name = "Raipur", DistrictId = 15 },
            new() { Id = 114, Name = "Ramganj", DistrictId = 15 },
            new() { Id = 115, Name = "Ramgati", DistrictId = 15 },

            // Noakhali
            new() { Id = 116, Name = "Begumganj", DistrictId = 16 },
            new() { Id = 117, Name = "Chatkhil", DistrictId = 16 },
            new() { Id = 118, Name = "Companiganj", DistrictId = 16 },
            new() { Id = 119, Name = "Hatiya", DistrictId = 16 },
            new() { Id = 120, Name = "Noakhali Sadar", DistrictId = 16 },
            new() { Id = 121, Name = "Senbagh", DistrictId = 16 },
            new() { Id = 122, Name = "Sonaimuri", DistrictId = 16 },
            new() { Id = 123, Name = "Subarnachar", DistrictId = 16 },

            // Rangamati
            new() { Id = 124, Name = "Bagaichhari", DistrictId = 17 },
            new() { Id = 125, Name = "Barkal", DistrictId = 17 },
            new() { Id = 126, Name = "Juraichhari", DistrictId = 17 },
            new() { Id = 127, Name = "Kaptai", DistrictId = 17 },
            new() { Id = 128, Name = "Langadu", DistrictId = 17 },
            new() { Id = 129, Name = "Naniarchar", DistrictId = 17 },
            new() { Id = 130, Name = "Rajasthali", DistrictId = 17 },
            new() { Id = 131, Name = "Rangamati Sadar", DistrictId = 17 },

            // Dhaka
            new() { Id = 132, Name = "Dhanmondi", DistrictId = 18 },
            new() { Id = 133, Name = "Gulshan", DistrictId = 18 },
            new() { Id = 134, Name = "Uttara", DistrictId = 18 },
            new() { Id = 135, Name = "Mirpur", DistrictId = 18 },
            new() { Id = 136, Name = "Mohammadpur", DistrictId = 18 },
            new() { Id = 137, Name = "Tejgaon", DistrictId = 18 },
            new() { Id = 138, Name = "Motijheel", DistrictId = 18 },
            new() { Id = 139, Name = "Demra", DistrictId = 18 },
            new() { Id = 140, Name = "Keraniganj", DistrictId = 18 },
            new() { Id = 141, Name = "Nawabganj", DistrictId = 18 },
            new() { Id = 142, Name = "Savar", DistrictId = 18 },
            new() { Id = 143, Name = "Ashulia", DistrictId = 18 },

            // Faridpur
            new() { Id = 144, Name = "Alfadanga", DistrictId = 19 },
            new() { Id = 145, Name = "Bhanga", DistrictId = 19 },
            new() { Id = 146, Name = "Boalmari", DistrictId = 19 },
            new() { Id = 147, Name = "Char Bhadrasan", DistrictId = 19 },
            new() { Id = 148, Name = "Faridpur Sadar", DistrictId = 19 },
            new() { Id = 149, Name = "Madhukhali", DistrictId = 19 },
            new() { Id = 150, Name = "Nagarkanda", DistrictId = 19 },
            new() { Id = 151, Name = "Sadarpur", DistrictId = 19 },
            new() { Id = 152, Name = "Saltha", DistrictId = 19 },

            // Gazipur
            new() { Id = 153, Name = "Gazipur Sadar", DistrictId = 20 },
            new() { Id = 154, Name = "Kaliakair", DistrictId = 20 },
            new() { Id = 155, Name = "Kaliganj", DistrictId = 20 },
            new() { Id = 156, Name = "Kapasia", DistrictId = 20 },
            new() { Id = 157, Name = "Sreepur", DistrictId = 20 },

            // Gopalganj
            new() { Id = 158, Name = "Gopalganj Sadar", DistrictId = 21 },
            new() { Id = 159, Name = "Kashiani", DistrictId = 21 },
            new() { Id = 160, Name = "Kotalipara", DistrictId = 21 },
            new() { Id = 161, Name = "Muksudpur", DistrictId = 21 },
            new() { Id = 162, Name = "Tungipara", DistrictId = 21 },

            // Kishoreganj
            new() { Id = 163, Name = "Austagram", DistrictId = 22 },
            new() { Id = 164, Name = "Bajitpur", DistrictId = 22 },
            new() { Id = 165, Name = "Bhairab", DistrictId = 22 },
            new() { Id = 166, Name = "Hossainpur", DistrictId = 22 },
            new() { Id = 167, Name = "Itna", DistrictId = 22 },
            new() { Id = 168, Name = "Karimganj", DistrictId = 22 },
            new() { Id = 169, Name = "Kishoreganj Sadar", DistrictId = 22 },
            new() { Id = 170, Name = "Kuliarchar", DistrictId = 22 },
            new() { Id = 171, Name = "Mithamain", DistrictId = 22 },
            new() { Id = 172, Name = "Nikli", DistrictId = 22 },
            new() { Id = 173, Name = "Pakundia", DistrictId = 22 },
            new() { Id = 174, Name = "Tarail", DistrictId = 22 },

            // Madaripur
            new() { Id = 175, Name = "Dasherkhara", DistrictId = 23 },
            new() { Id = 176, Name = "Kalkini", DistrictId = 23 },
            new() { Id = 177, Name = "Madaripur Sadar", DistrictId = 23 },
            new() { Id = 178, Name = "Rajoir", DistrictId = 23 },
            new() { Id = 179, Name = "Shibchar", DistrictId = 23 },

            // Manikganj
            new() { Id = 180, Name = "Daulatpur", DistrictId = 24 },
            new() { Id = 181, Name = "Ghior", DistrictId = 24 },
            new() { Id = 182, Name = "Harirampur", DistrictId = 24 },
            new() { Id = 183, Name = "Manikganj Sadar", DistrictId = 24 },
            new() { Id = 184, Name = "Saturia", DistrictId = 24 },
            new() { Id = 185, Name = "Shivalaya", DistrictId = 24 },
            new() { Id = 186, Name = "Singair", DistrictId = 24 },

            // Munshiganj
            new() { Id = 187, Name = "Gazaria", DistrictId = 25 },
            new() { Id = 188, Name = "Lohajang", DistrictId = 25 },
            new() { Id = 189, Name = "Munshiganj Sadar", DistrictId = 25 },
            new() { Id = 190, Name = "Sirajdikhan", DistrictId = 25 },
            new() { Id = 191, Name = "Sreenagar", DistrictId = 25 },
            new() { Id = 192, Name = "Tongibari", DistrictId = 25 },

            // Narayanganj
            new() { Id = 193, Name = "Araihazar", DistrictId = 26 },
            new() { Id = 194, Name = "Bandar", DistrictId = 26 },
            new() { Id = 195, Name = "Fatullah", DistrictId = 26 },
            new() { Id = 196, Name = "Narayanganj Sadar", DistrictId = 26 },
            new() { Id = 197, Name = "Sonargaon", DistrictId = 26 },

            // Narsingdi
            new() { Id = 198, Name = "Belabo", DistrictId = 27 },
            new() { Id = 199, Name = "Monohardi", DistrictId = 27 },
            new() { Id = 200, Name = "Narsingdi Sadar", DistrictId = 27 },
            new() { Id = 201, Name = "Palash", DistrictId = 27 },
            new() { Id = 202, Name = "Raipura", DistrictId = 27 },
            new() { Id = 203, Name = "Shibpur", DistrictId = 27 },

            // Rajbari
            new() { Id = 204, Name = "Baliakandi", DistrictId = 28 },
            new() { Id = 205, Name = "Goalandaga", DistrictId = 28 },
            new() { Id = 206, Name = "Pangsha", DistrictId = 28 },
            new() { Id = 207, Name = "Rajbari Sadar", DistrictId = 28 },
            new() { Id = 208, Name = "Kalukhali", DistrictId = 28 },

            // Shariatpur
            new() { Id = 209, Name = "Bhedarganj", DistrictId = 29 },
            new() { Id = 210, Name = "Damudya", DistrictId = 29 },
            new() { Id = 211, Name = "Gosairhat", DistrictId = 29 },
            new() { Id = 212, Name = "Naria", DistrictId = 29 },
            new() { Id = 213, Name = "Shariatpur Sadar", DistrictId = 29 },
            new() { Id = 214, Name = "Zajira", DistrictId = 29 },

            // Tangail
            new() { Id = 215, Name = "Basail", DistrictId = 30 },
            new() { Id = 216, Name = "Bhuapur", DistrictId = 30 },
            new() { Id = 217, Name = "Delduar", DistrictId = 30 },
            new() { Id = 218, Name = "Ghatail", DistrictId = 30 },
            new() { Id = 219, Name = "Gopalpur", DistrictId = 30 },
            new() { Id = 220, Name = "Kalihati", DistrictId = 30 },
            new() { Id = 221, Name = "Madhupur", DistrictId = 30 },
            new() { Id = 222, Name = "Mirzapur", DistrictId = 30 },
            new() { Id = 223, Name = "Nagarpur", DistrictId = 30 },
            new() { Id = 224, Name = "Sherpur", DistrictId = 30 },
            new() { Id = 225, Name = "Tangail Sadar", DistrictId = 30 },

            // Bagerhat
            new() { Id = 226, Name = "Bagerhat Sadar", DistrictId = 31 },
            new() { Id = 227, Name = "Chitalmari", DistrictId = 31 },
            new() { Id = 228, Name = "Fakirhat", DistrictId = 31 },
            new() { Id = 229, Name = "Kachua", DistrictId = 31 },
            new() { Id = 230, Name = "Mollahat", DistrictId = 31 },
            new() { Id = 231, Name = "Mongla", DistrictId = 31 },
            new() { Id = 232, Name = "Rampal", DistrictId = 31 },
            new() { Id = 233, Name = "Sarankhola", DistrictId = 31 },

            // Chuadanga
            new() { Id = 234, Name = "Alamdanga", DistrictId = 32 },
            new() { Id = 235, Name = "Chuadanga Sadar", DistrictId = 32 },
            new() { Id = 236, Name = "Damurhuda", DistrictId = 32 },
            new() { Id = 237, Name = "Jibannagar", DistrictId = 32 },

            // Jashore
            new() { Id = 238, Name = "Abhaynagar", DistrictId = 33 },
            new() { Id = 239, Name = "Bagherpara", DistrictId = 33 },
            new() { Id = 240, Name = "Chougachha", DistrictId = 33 },
            new() { Id = 241, Name = "Jhikargacha", DistrictId = 33 },
            new() { Id = 242, Name = "Jashore Sadar", DistrictId = 33 },
            new() { Id = 243, Name = "Keshabpur", DistrictId = 33 },
            new() { Id = 244, Name = "Monirampur", DistrictId = 33 },
            new() { Id = 245, Name = "Sharsha", DistrictId = 33 },

            // Jhenaidah
            new() { Id = 246, Name = "Harinakunda", DistrictId = 34 },
            new() { Id = 247, Name = "Jhenaidah Sadar", DistrictId = 34 },
            new() { Id = 248, Name = "Kaliganj", DistrictId = 34 },
            new() { Id = 249, Name = "Kotchandpur", DistrictId = 34 },
            new() { Id = 250, Name = "Maheshpur", DistrictId = 34 },
            new() { Id = 251, Name = "Shailkupa", DistrictId = 34 },

            // Khulna
            new() { Id = 252, Name = "Batiaghata", DistrictId = 35 },
            new() { Id = 253, Name = "Dacope", DistrictId = 35 },
            new() { Id = 254, Name = "Dumuria", DistrictId = 35 },
            new() { Id = 255, Name = "Dighalia", DistrictId = 35 },
            new() { Id = 256, Name = "Koyra", DistrictId = 35 },
            new() { Id = 257, Name = "Paikgachha", DistrictId = 35 },
            new() { Id = 258, Name = "Phultala", DistrictId = 35 },
            new() { Id = 259, Name = "Rupsa", DistrictId = 35 },
            new() { Id = 260, Name = "Terokhada", DistrictId = 35 },

            // Kushtia
            new() { Id = 261, Name = "Bheramara", DistrictId = 36 },
            new() { Id = 262, Name = "Daulatpur", DistrictId = 36 },
            new() { Id = 263, Name = "Khoksa", DistrictId = 36 },
            new() { Id = 264, Name = "Kumarkhali", DistrictId = 36 },
            new() { Id = 265, Name = "Kushtia Sadar", DistrictId = 36 },
            new() { Id = 266, Name = "Mirpur", DistrictId = 36 },

            // Magura
            new() { Id = 267, Name = "Magura Sadar", DistrictId = 37 },
            new() { Id = 268, Name = "Mohammadpur", DistrictId = 37 },
            new() { Id = 269, Name = "Shalikha", DistrictId = 37 },
            new() { Id = 270, Name = "Sreepur", DistrictId = 37 },

            // Meherpur
            new() { Id = 271, Name = "Gangni", DistrictId = 38 },
            new() { Id = 272, Name = "Meherpur Sadar", DistrictId = 38 },
            new() { Id = 273, Name = "Mujibnagar", DistrictId = 38 },

            // Narail
            new() { Id = 274, Name = "Kalia", DistrictId = 39 },
            new() { Id = 275, Name = "Lohagara", DistrictId = 39 },
            new() { Id = 276, Name = "Narail Sadar", DistrictId = 39 },
            new() { Id = 277, Name = "Optani", DistrictId = 39 },

            // Satkhira
            new() { Id = 278, Name = "Assasuni", DistrictId = 40 },
            new() { Id = 279, Name = "Debhata", DistrictId = 40 },
            new() { Id = 280, Name = "Kalaroa", DistrictId = 40 },
            new() { Id = 281, Name = "Kaliganj", DistrictId = 40 },
            new() { Id = 282, Name = "Satkhira Sadar", DistrictId = 40 },
            new() { Id = 283, Name = "Tala", DistrictId = 40 },

            // Jamalpur
            new() { Id = 284, Name = "Bakshiganj", DistrictId = 41 },
            new() { Id = 285, Name = "Dewanganj", DistrictId = 41 },
            new() { Id = 286, Name = "Islampur", DistrictId = 41 },
            new() { Id = 287, Name = "Jamalpur Sadar", DistrictId = 41 },
            new() { Id = 288, Name = "Madarganj", DistrictId = 41 },
            new() { Id = 289, Name = "Melandaha", DistrictId = 41 },
            new() { Id = 290, Name = "Sarishabari", DistrictId = 41 },

            // Mymensingh
            new() { Id = 291, Name = "Bhaluka", DistrictId = 42 },
            new() { Id = 292, Name = "Dobaura", DistrictId = 42 },
            new() { Id = 293, Name = "Gaffargaon", DistrictId = 42 },
            new() { Id = 294, Name = "Ishwarganj", DistrictId = 42 },
            new() { Id = 295, Name = "Mymensingh Sadar", DistrictId = 42 },
            new() { Id = 296, Name = "Muktagachha", DistrictId = 42 },
            new() { Id = 297, Name = "Phulpur", DistrictId = 42 },
            new() { Id = 298, Name = "Trishal", DistrictId = 42 },

            // Netrokona
            new() { Id = 299, Name = "Atpara", DistrictId = 43 },
            new() { Id = 300, Name = "Barhatta", DistrictId = 43 },
            new() { Id = 301, Name = "Durgapur", DistrictId = 43 },
            new() { Id = 302, Name = "Khaliajuri", DistrictId = 43 },
            new() { Id = 303, Name = "Madan", DistrictId = 43 },
            new() { Id = 304, Name = "Mohanganj", DistrictId = 43 },
            new() { Id = 305, Name = "Netrokona Sadar", DistrictId = 43 },
            new() { Id = 306, Name = "Purbadhala", DistrictId = 43 },

            // Sherpur
            new() { Id = 307, Name = "Jhenaigati", DistrictId = 44 },
            new() { Id = 308, Name = "Nakla", DistrictId = 44 },
            new() { Id = 309, Name = "Nalitabari", DistrictId = 44 },
            new() { Id = 310, Name = "Sherpur Sadar", DistrictId = 44 },
            new() { Id = 311, Name = "Sribardi", DistrictId = 44 },

            // Bogura
            new() { Id = 312, Name = "Adamdighi", DistrictId = 45 },
            new() { Id = 313, Name = "Bogura Sadar", DistrictId = 45 },
            new() { Id = 314, Name = "Dhunat", DistrictId = 45 },
            new() { Id = 315, Name = "Dhupchanchia", DistrictId = 45 },
            new() { Id = 316, Name = "Gabtali", DistrictId = 45 },
            new() { Id = 317, Name = "Kahaloo", DistrictId = 45 },
            new() { Id = 318, Name = "Nandigram", DistrictId = 45 },
            new() { Id = 319, Name = "Sariakandi", DistrictId = 45 },
            new() { Id = 320, Name = "Shajahanpur", DistrictId = 45 },
            new() { Id = 321, Name = "Shibganj", DistrictId = 45 },
            new() { Id = 322, Name = "Sonatala", DistrictId = 45 },

            // Joypurhat
            new() { Id = 323, Name = "Akkelpur", DistrictId = 46 },
            new() { Id = 324, Name = "Joypurhat Sadar", DistrictId = 46 },
            new() { Id = 325, Name = "Kalai", DistrictId = 46 },
            new() { Id = 326, Name = "Khetlal", DistrictId = 46 },
            new() { Id = 327, Name = "Panchbibi", DistrictId = 46 },

            // Naogaon
            new() { Id = 328, Name = "Atrai", DistrictId = 47 },
            new() { Id = 329, Name = "Badalgachhi", DistrictId = 47 },
            new() { Id = 330, Name = "Dhamoirhat", DistrictId = 47 },
            new() { Id = 331, Name = "Manda", DistrictId = 47 },
            new() { Id = 332, Name = "Mahadevpur", DistrictId = 47 },
            new() { Id = 333, Name = "Naogaon Sadar", DistrictId = 47 },
            new() { Id = 334, Name = "Niamatpur", DistrictId = 47 },
            new() { Id = 335, Name = "Patnitala", DistrictId = 47 },
            new() { Id = 336, Name = "Porsha", DistrictId = 47 },
            new() { Id = 337, Name = "Raninagar", DistrictId = 47 },
            new() { Id = 338, Name = "Sapahar", DistrictId = 47 },

            // Natore
            new() { Id = 339, Name = "Bagatipara", DistrictId = 48 },
            new() { Id = 340, Name = "Baraigram", DistrictId = 48 },
            new() { Id = 341, Name = "Gurudaspur", DistrictId = 48 },
            new() { Id = 342, Name = "Lalpur", DistrictId = 48 },
            new() { Id = 343, Name = "Natore Sadar", DistrictId = 48 },
            new() { Id = 344, Name = "Singra", DistrictId = 48 },

            // Chapainawabganj
            new() { Id = 345, Name = "Bholahat", DistrictId = 49 },
            new() { Id = 346, Name = "Chapainawabganj Sadar", DistrictId = 49 },
            new() { Id = 347, Name = "Gomastapur", DistrictId = 49 },
            new() { Id = 348, Name = "Nachol", DistrictId = 49 },
            new() { Id = 349, Name = "Rohanpur", DistrictId = 49 },
            new() { Id = 350, Name = "Shibganj", DistrictId = 49 },

            // Pabna
            new() { Id = 351, Name = "Atgharia", DistrictId = 50 },
            new() { Id = 352, Name = "Bera", DistrictId = 50 },
            new() { Id = 353, Name = "Bhangura", DistrictId = 50 },
            new() { Id = 354, Name = "Chatmohar", DistrictId = 50 },
            new() { Id = 355, Name = "Ishwardi", DistrictId = 50 },
            new() { Id = 356, Name = "Pabna Sadar", DistrictId = 50 },
            new() { Id = 357, Name = "Santhia", DistrictId = 50 },
            new() { Id = 358, Name = "Sujanagar", DistrictId = 50 },

            // Rajshahi
            new() { Id = 359, Name = "Bagha", DistrictId = 51 },
            new() { Id = 360, Name = "Bagmara", DistrictId = 51 },
            new() { Id = 361, Name = "Charghat", DistrictId = 51 },
            new() { Id = 362, Name = "Durgapur", DistrictId = 51 },
            new() { Id = 363, Name = "Godagari", DistrictId = 51 },
            new() { Id = 364, Name = "Mohanpur", DistrictId = 51 },
            new() { Id = 365, Name = "Paba", DistrictId = 51 },
            new() { Id = 366, Name = "Putia", DistrictId = 51 },
            new() { Id = 367, Name = "Rajshahi Sadar", DistrictId = 51 },
            new() { Id = 368, Name = "Tanore", DistrictId = 51 },

            // Sirajganj
            new() { Id = 369, Name = "Belkuchi", DistrictId = 52 },
            new() { Id = 370, Name = "Kazipur", DistrictId = 52 },
            new() { Id = 371, Name = "Raiganj", DistrictId = 52 },
            new() { Id = 372, Name = "Shahjadpur", DistrictId = 52 },
            new() { Id = 373, Name = "Sirajganj Sadar", DistrictId = 52 },
            new() { Id = 374, Name = "Tarash", DistrictId = 52 },
            new() { Id = 375, Name = "Ullapara", DistrictId = 52 },

            // Dinajpur
            new() { Id = 376, Name = "Birampur", DistrictId = 53 },
            new() { Id = 377, Name = "Birganj", DistrictId = 53 },
            new() { Id = 378, Name = "Biral", DistrictId = 53 },
            new() { Id = 379, Name = "Chirirbandar", DistrictId = 53 },
            new() { Id = 380, Name = "Dinajpur Sadar", DistrictId = 53 },
            new() { Id = 381, Name = "Ghoraghat", DistrictId = 53 },
            new() { Id = 382, Name = "Hakimpur", DistrictId = 53 },
            new() { Id = 383, Name = "Kaharol", DistrictId = 53 },
            new() { Id = 384, Name = "Nawabganj", DistrictId = 53 },
            new() { Id = 385, Name = "Parbatipur", DistrictId = 53 },

            // Gaibandha
            new() { Id = 386, Name = "Fulchhari", DistrictId = 54 },
            new() { Id = 387, Name = "Gaibandha Sadar", DistrictId = 54 },
            new() { Id = 388, Name = "Gobindaganj", DistrictId = 54 },
            new() { Id = 389, Name = "Palashbari", DistrictId = 54 },
            new() { Id = 390, Name = "Sadullapur", DistrictId = 54 },
            new() { Id = 391, Name = "Saghata", DistrictId = 54 },
            new() { Id = 392, Name = "Sundarganj", DistrictId = 54 },

            // Kurigram
            new() { Id = 393, Name = "Bhurungamari", DistrictId = 55 },
            new() { Id = 394, Name = "Charrajibpur", DistrictId = 55 },
            new() { Id = 395, Name = "Fulbari", DistrictId = 55 },
            new() { Id = 396, Name = "Kurigram Sadar", DistrictId = 55 },
            new() { Id = 397, Name = "Nageshwari", DistrictId = 55 },
            new() { Id = 398, Name = "Rajarhat", DistrictId = 55 },
            new() { Id = 399, Name = "Raumari", DistrictId = 55 },
            new() { Id = 400, Name = "Ulipur", DistrictId = 55 },

            // Lalmonirhat
            new() { Id = 401, Name = "Aditmari", DistrictId = 56 },
            new() { Id = 402, Name = "Hatibandha", DistrictId = 56 },
            new() { Id = 403, Name = "Kaliganj", DistrictId = 56 },
            new() { Id = 404, Name = "Lalmonirhat Sadar", DistrictId = 56 },
            new() { Id = 405, Name = "Patgram", DistrictId = 56 },

            // Nilphamari
            new() { Id = 406, Name = "Dimla", DistrictId = 57 },
            new() { Id = 407, Name = "Domar", DistrictId = 57 },
            new() { Id = 408, Name = "Jaldhaka", DistrictId = 57 },
            new() { Id = 409, Name = "Kishoreganj", DistrictId = 57 },
            new() { Id = 410, Name = "Nilphamari Sadar", DistrictId = 57 },
            new() { Id = 411, Name = "Sadar", DistrictId = 57 },

            // Panchagarh
            new() { Id = 412, Name = "Atwari", DistrictId = 58 },
            new() { Id = 413, Name = "Boda", DistrictId = 58 },
            new() { Id = 414, Name = "Debiganj", DistrictId = 58 },
            new() { Id = 415, Name = "Panchagarh Sadar", DistrictId = 58 },
            new() { Id = 416, Name = "Tetulia", DistrictId = 58 },

            // Rangpur
            new() { Id = 417, Name = "Badarganj", DistrictId = 59 },
            new() { Id = 418, Name = "Gangachhara", DistrictId = 59 },
            new() { Id = 419, Name = "Kawnia", DistrictId = 59 },
            new() { Id = 420, Name = "Kaunia", DistrictId = 59 },
            new() { Id = 421, Name = "Mithapukur", DistrictId = 59 },
            new() { Id = 422, Name = "Pirgachha", DistrictId = 59 },
            new() { Id = 423, Name = "Pirganj", DistrictId = 59 },
            new() { Id = 424, Name = "Rangpur Sadar", DistrictId = 59 },
            new() { Id = 425, Name = "Taraganj", DistrictId = 59 },

            // Thakurgaon
            new() { Id = 426, Name = "Baliadangi", DistrictId = 60 },
            new() { Id = 427, Name = "Haripur", DistrictId = 60 },
            new() { Id = 428, Name = "Pirganj", DistrictId = 60 },
            new() { Id = 429, Name = "Ranisankail", DistrictId = 60 },
            new() { Id = 430, Name = "Thakurgaon Sadar", DistrictId = 60 },

            // Habiganj
            new() { Id = 431, Name = "Ajmiriganj", DistrictId = 61 },
            new() { Id = 432, Name = "Bahubal", DistrictId = 61 },
            new() { Id = 433, Name = "Baniachong", DistrictId = 61 },
            new() { Id = 434, Name = "Chunarughat", DistrictId = 61 },
            new() { Id = 435, Name = "Habiganj Sadar", DistrictId = 61 },
            new() { Id = 436, Name = "Lakhai", DistrictId = 61 },
            new() { Id = 437, Name = "Madhabpur", DistrictId = 61 },
            new() { Id = 438, Name = "Nabiganj", DistrictId = 61 },
            new() { Id = 439, Name = "Shaista", DistrictId = 61 },

            // Moulvibazar
            new() { Id = 440, Name = "Barlekha", DistrictId = 62 },
            new() { Id = 441, Name = "Juri", DistrictId = 62 },
            new() { Id = 442, Name = "Kamalganj", DistrictId = 62 },
            new() { Id = 443, Name = "Kulaura", DistrictId = 62 },
            new() { Id = 444, Name = "Moulvibazar Sadar", DistrictId = 62 },
            new() { Id = 445, Name = "Rajnagar", DistrictId = 62 },
            new() { Id = 446, Name = "Sreemangal", DistrictId = 62 },

            // Sunamganj
            new() { Id = 447, Name = "Bishwamvarpur", DistrictId = 63 },
            new() { Id = 448, Name = "Chhatak", DistrictId = 63 },
            new() { Id = 449, Name = "Derai", DistrictId = 63 },
            new() { Id = 450, Name = "Dharmapasha", DistrictId = 63 },
            new() { Id = 451, Name = "Jagannathpur", DistrictId = 63 },
            new() { Id = 452, Name = "Jamalganj", DistrictId = 63 },
            new() { Id = 453, Name = "Sullah", DistrictId = 63 },
            new() { Id = 454, Name = "Sunamganj Sadar", DistrictId = 63 },
            new() { Id = 455, Name = "Tahirpur", DistrictId = 63 },

            // Sylhet
            new() { Id = 456, Name = "Balaganj", DistrictId = 64 },
            new() { Id = 457, Name = "Companiganj", DistrictId = 64 },
            new() { Id = 458, Name = "Fenchuganj", DistrictId = 64 },
            new() { Id = 459, Name = "Golapganj", DistrictId = 64 },
            new() { Id = 460, Name = "Gowainghat", DistrictId = 64 },
            new() { Id = 461, Name = "Jaintiapur", DistrictId = 64 },
            new() { Id = 462, Name = "Kanaighat", DistrictId = 64 },
            new() { Id = 463, Name = "Zakiganj", DistrictId = 64 },
            new() { Id = 464, Name = "Sylhet Sadar", DistrictId = 64 }
        };

    public static List<AddressDistrict> GetDistrictsByDivision(int divisionId)
        => Districts.Where(d => d.DivisionId == divisionId).OrderBy(d => d.Name).ToList();

    public static List<AddressUpazila> GetUpazilasByDistrict(int districtId)
        => Upazilas.Where(u => u.DistrictId == districtId).OrderBy(u => u.Name).ToList();
}
