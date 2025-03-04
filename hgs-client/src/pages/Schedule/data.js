export const teacherData = [
  { teacher_id: 1, teacher_name: "Nguyễn Văn A" },
  { teacher_id: 2, teacher_name: "Trần Thị B" },
  { teacher_id: 3, teacher_name: "Lê Văn C" },
  { teacher_id: 4, teacher_name: "Phạm Thị D" },
  { teacher_id: 5, teacher_name: "Hoàng Văn E" },
  { teacher_id: 6, teacher_name: "Dương Văn F" },
  { teacher_id: 7, teacher_name: "Phan Thị G" },
  { teacher_id: 8, teacher_name: "Đoàn Thị H" },
  { teacher_id: 9, teacher_name: "Lý Thị I" },
  { teacher_id: 10, teacher_name: "Tôn Thị J" },
  { teacher_id: 11, teacher_name: "NguyễnNguyễn Văn K" },
  { teacher_id: 12, teacher_name: "Trần Thị L" },
  { teacher_id: 13, teacher_name: "Lê Văn M" },
  { teacher_id: 14, teacher_name: "Tôn Thị N" },
  { teacher_id: 15, teacher_name: "NguyễnNguyễn Văn O" },
  { teacher_id: 16, teacher_name: "Trần Thị P" },
  { teacher_id: 17, teacher_name: "Lê Văn Q" },
  { teacher_id: 18, teacher_name: "Tôn Thị R" },
  { teacher_id: 19, teacher_name: "Nguyễn Văn S" },
  { teacher_id: 20, teacher_name: "Trần Thị T" },
  { teacher_id: 21, teacher_name: "Lê Văn U" },
];

export const subjectData = [
  { subject_Id: 1, subject_code: "TOAN", subject_name: "Toán học" },
  { subject_Id: 2, subject_code: "VAN", subject_name: "Ngữ văn" },
  { subject_Id: 3, subject_code: "ANH", subject_name: "Tiếng Anh" },
  { subject_Id: 4, subject_code: "LY", subject_name: "Vật lý" },
  { subject_Id: 5, subject_code: "HOA", subject_name: "Hóa học" },
  { subject_Id: 6, subject_code: "SINH", subject_name: "Sinh học" },
  { subject_Id: 7, subject_code: "SU", subject_name: "Lịch sử" },
  { subject_Id: 8, subject_code: "DIA", subject_name: "Địa lý" },
  { subject_Id: 9, subject_code: "GDCD", subject_name: "Giáo dục công dân" },
  { subject_Id: 10, subject_code: "CN", subject_name: "Công nghệ" },
  { subject_Id: 11, subject_code: "TIN", subject_name: "Tin học" },
  { subject_Id: 12, subject_code: "NHAC", subject_name: "Âm nhạc" },
  { subject_Id: 13, subject_code: "MYTHUAT", subject_name: "Mỹ thuật" },
  { subject_Id: 14, subject_code: "TD", subject_name: "Thể dục" },
  { subject_Id: 15, subject_code: "SHCD", subject_name: "Sinh hoạt chủ nhiệm" },
];

export const scheduleData = {
  "Grade 6": {
    "6A": {
      Monday: {
        Morning: [
          { period: 1, subject_Id: "1", teacher_id: "3" },
          { period: 2, subject_Id: "3", teacher_id: "5" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "1", teacher_id: "9" },
          { period: 5, subject_Id: "4", teacher_id: "6" },
        ],
        Afternoon: [{ period: 1, subject_Id: "7", teacher_id: "4" }],
      },
      Tuesday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "1" },
          { period: 2, subject_Id: "3", teacher_id: "5" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "4", teacher_id: "9" },
        ],
        Afternoon: [{ period: 1, subject_Id: "4", teacher_id: "4" }],
      },
      Wednesday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "2" },
          { period: 2, subject_Id: "7", teacher_id: "4" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "3", teacher_id: "5" },
        ],
        Afternoon: [],
      },
      Thursday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "1" },
          { period: 2, subject_Id: "4", teacher_id: "9" },
          { period: 3, subject_Id: "7", teacher_id: "4" },
          { period: 4, subject_Id: "4", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Friday: {
        Morning: [
          { period: 1, subject_Id: "1", teacher_id: "3" },
          { period: 2, subject_Id: "4", teacher_id: "1" },
          { period: 3, subject_Id: "3", teacher_id: "5" },
          { period: 4, subject_Id: "4", teacher_id: "9" },
        ],
        Afternoon: [{ period: 1, subject_Id: "4", teacher_id: "2" }],
      },
      Saturday: {
        Morning: [
          { period: 1, subject_Id: "3", teacher_id: "5" },
          { period: 2, subject_Id: "4", teacher_id: "9" },
          { period: 3, subject_Id: "7", teacher_id: "4" },
          { period: 4, subject_Id: "4", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Sunday: {
        Morning: [],
        Afternoon: [],
      },
    },
    "6B": {
      Monday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "1" },
          { period: 2, subject_Id: "4", teacher_id: "23" },
          { period: 3, subject_Id: "7", teacher_id: "2" },
          { period: 4, subject_Id: "3", teacher_id: "5" },
        ],
        Afternoon: [{ period: 1, subject_Id: "4", teacher_id: "2" }],
      },
      Tuesday: {
        Morning: [
          { period: 1, subject_Id: "1", teacher_id: "3" },
          { period: 2, subject_Id: "4", teacher_id: "1" },
          { period: 3, subject_Id: "4", teacher_id: "9" },
          { period: 4, subject_Id: "7", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Wednesday: {
        Morning: [
          { period: 1, subject_Id: "7", teacher_id: "4" },
          { period: 2, subject_Id: "4", teacher_id: "4" },
          { period: 3, subject_Id: "3", teacher_id: "5" },
          { period: 4, subject_Id: "1", teacher_id: "8" },
        ],
        Afternoon: [],
      },
      Thursday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "1" },
          { period: 2, subject_Id: "4", teacher_id: "9" },
          { period: 3, subject_Id: "7", teacher_id: "4" },
          { period: 4, subject_Id: "4", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Friday: {
        Morning: [
          { period: 1, subject_Id: "1", teacher_id: "3" },
          { period: 2, subject_Id: "4", teacher_id: "1" },
          { period: 3, subject_Id: "3", teacher_id: "5" },
          { period: 4, subject_Id: "4", teacher_id: "9" },
        ],
        Afternoon: [{ period: 1, subject_Id: "4", teacher_id: "2" }],
      },
      Saturday: {
        Morning: [
          { period: 1, subject_Id: "3", teacher_id: "5" },
          { period: 2, subject_Id: "4", teacher_id: "9" },
          { period: 3, subject_Id: "7", teacher_id: "4" },
          { period: 4, subject_Id: "4", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Sunday: {
        Morning: [],
        Afternoon: [],
      },
    },
    "6C": {
      Monday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "1" },
          { period: 2, subject_Id: "4", teacher_id: "23" },
          { period: 3, subject_Id: "7", teacher_id: "2" },
          { period: 4, subject_Id: "3", teacher_id: "5" },
        ],
        Afternoon: [{ period: 1, subject_Id: "4", teacher_id: "2" }],
      },
      Tuesday: {
        Morning: [
          { period: 1, subject_Id: "1", teacher_id: "3" },
          { period: 2, subject_Id: "4", teacher_id: "1" },
          { period: 3, subject_Id: "4", teacher_id: "9" },
          { period: 4, subject_Id: "7", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Wednesday: {
        Morning: [
          { period: 1, subject_Id: "7", teacher_id: "4" },
          { period: 2, subject_Id: "4", teacher_id: "4" },
          { period: 3, subject_Id: "3", teacher_id: "5" },
          { period: 4, subject_Id: "1", teacher_id: "8" },
        ],
        Afternoon: [],
      },
      Thursday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "1" },
          { period: 2, subject_Id: "4", teacher_id: "9" },
          { period: 3, subject_Id: "7", teacher_id: "4" },
          { period: 4, subject_Id: "4", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Friday: {
        Morning: [
          { period: 1, subject_Id: "1", teacher_id: "3" },
          { period: 2, subject_Id: "4", teacher_id: "1" },
          { period: 3, subject_Id: "3", teacher_id: "5" },
          { period: 4, subject_Id: "4", teacher_id: "9" },
        ],
        Afternoon: [{ period: 1, subject_Id: "4", teacher_id: "2" }],
      },
      Saturday: {
        Morning: [
          { period: 1, subject_Id: "3", teacher_id: "5" },
          { period: 2, subject_Id: "4", teacher_id: "9" },
          { period: 3, subject_Id: "7", teacher_id: "4" },
          { period: 4, subject_Id: "4", teacher_id: "4" },
        ],
        Afternoon: [],
      },
    },
  },
  "Grade 7": {
    "7A": {
      Monday: {
        Morning: [
          { period: 1, subject_Id: "1", teacher_id: "3" },
          { period: 2, subject_Id: "3", teacher_id: "5" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "1", teacher_id: "9" },
          { period: 5, subject_Id: "4", teacher_id: "6" },
        ],
        Afternoon: [{ period: 1, subject_Id: "7", teacher_id: "4" }],
      },
      Tuesday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "1" },
          { period: 2, subject_Id: "3", teacher_id: "5" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "4", teacher_id: "9" },
        ],
        Afternoon: [{ period: 1, subject_Id: "4", teacher_id: "4" }],
      },
      Wednesday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "12" },
          { period: 2, subject_Id: "7", teacher_id: "14" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "3", teacher_id: "5" },
        ],
        Afternoon: [],
      },
      Thursday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "1" },
          { period: 2, subject_Id: "4", teacher_id: "9" },
          { period: 3, subject_Id: "7", teacher_id: "4" },
          { period: 4, subject_Id: "4", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Friday: {
        Morning: [
          { period: 1, subject_Id: "1", teacher_id: "3" },
          { period: 2, subject_Id: "4", teacher_id: "1" },
          { period: 3, subject_Id: "3", teacher_id: "5" },
          { period: 4, subject_Id: "4", teacher_id: "9" },
        ],
        Afternoon: [{ period: 1, subject_Id: "4", teacher_id: "2" }],
      },
      Saturday: {
        Morning: [
          { period: 1, subject_Id: "3", teacher_id: "5" },
          { period: 2, subject_Id: "4", teacher_id: "9" },
          { period: 3, subject_Id: "7", teacher_id: "4" },
          { period: 4, subject_Id: "4", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Sunday: {
        Morning: [],
        Afternoon: [],
      },
    },
    "7B": {
      Monday: {
        Morning: [
          { period: 1, subject_Id: "1", teacher_id: "3" },
          { period: 2, subject_Id: "3", teacher_id: "5" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "1", teacher_id: "9" },
          { period: 5, subject_Id: "4", teacher_id: "6" },
        ],
        Afternoon: [{ period: 1, subject_Id: "7", teacher_id: "4" }],
      },
      Tuesday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "1" },
          { period: 2, subject_Id: "3", teacher_id: "5" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "4", teacher_id: "9" },
        ],
        Afternoon: [{ period: 1, subject_Id: "4", teacher_id: "4" }],
      },
      Wednesday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "12" },
          { period: 2, subject_Id: "7", teacher_id: "14" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "3", teacher_id: "5" },
        ],
        Afternoon: [],
      },
      Thursday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "1" },
          { period: 2, subject_Id: "4", teacher_id: "9" },
          { period: 3, subject_Id: "7", teacher_id: "4" },
          { period: 4, subject_Id: "4", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Friday: {
        Morning: [
          { period: 1, subject_Id: "1", teacher_id: "3" },
          { period: 2, subject_Id: "4", teacher_id: "1" },
          { period: 3, subject_Id: "3", teacher_id: "5" },
          { period: 4, subject_Id: "4", teacher_id: "9" },
        ],
        Afternoon: [{ period: 1, subject_Id: "4", teacher_id: "2" }],
      },
      Saturday: {
        Morning: [
          { period: 1, subject_Id: "3", teacher_id: "5" },
          { period: 2, subject_Id: "4", teacher_id: "9" },
          { period: 3, subject_Id: "7", teacher_id: "4" },
          { period: 4, subject_Id: "4", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Sunday: {
        Morning: [],
        Afternoon: [],
      },
    },
    "7C": {
      Monday: {
        Morning: [
          { period: 1, subject_Id: "1", teacher_id: "3" },
          { period: 2, subject_Id: "3", teacher_id: "5" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "1", teacher_id: "9" },
          { period: 5, subject_Id: "4", teacher_id: "6" },
        ],
        Afternoon: [{ period: 1, subject_Id: "7", teacher_id: "4" }],
      },
      Tuesday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "1" },
          { period: 2, subject_Id: "3", teacher_id: "5" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "4", teacher_id: "9" },
        ],
        Afternoon: [{ period: 1, subject_Id: "4", teacher_id: "4" }],
      },
      Wednesday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "12" },
          { period: 2, subject_Id: "7", teacher_id: "4" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "3", teacher_id: "5" },
        ],
        Afternoon: [],
      },
      Thursday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "1" },
          { period: 2, subject_Id: "4", teacher_id: "9" },
          { period: 3, subject_Id: "7", teacher_id: "4" },
          { period: 4, subject_Id: "4", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Friday: {
        Morning: [
          { period: 1, subject_Id: "1", teacher_id: "3" },
          { period: 2, subject_Id: "4", teacher_id: "1" },
          { period: 3, subject_Id: "3", teacher_id: "5" },
          { period: 4, subject_Id: "4", teacher_id: "9" },
        ],
        Afternoon: [{ period: 1, subject_Id: "4", teacher_id: "2" }],
      },
      Saturday: {
        Morning: [
          { period: 1, subject_Id: "3", teacher_id: "5" },
          { period: 2, subject_Id: "4", teacher_id: "9" },
          { period: 3, subject_Id: "7", teacher_id: "4" },
          { period: 4, subject_Id: "4", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Sunday: {
        Morning: [],
        Afternoon: [],
      },
    },
  },
  "Grade 8": {
    "8A": {
      Monday: {
        Morning: [
          { period: 1, subject_Id: "1", teacher_id: "3" },
          { period: 2, subject_Id: "3", teacher_id: "5" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "1", teacher_id: "9" },
          { period: 5, subject_Id: "4", teacher_id: "6" },
        ],
        Afternoon: [{ period: 1, subject_Id: "7", teacher_id: "4" }],
      },
      Tuesday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "1" },
          { period: 2, subject_Id: "3", teacher_id: "5" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "4", teacher_id: "9" },
        ],
        Afternoon: [{ period: 1, subject_Id: "4", teacher_id: "4" }],
      },
      Wednesday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "12" },
          { period: 2, subject_Id: "7", teacher_id: "14" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "3", teacher_id: "5" },
        ],
        Afternoon: [],
      },
      Thursday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "1" },
          { period: 2, subject_Id: "4", teacher_id: "9" },
          { period: 3, subject_Id: "7", teacher_id: "4" },
          { period: 4, subject_Id: "4", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Friday: {
        Morning: [
          { period: 1, subject_Id: "1", teacher_id: "3" },
          { period: 2, subject_Id: "4", teacher_id: "1" },
          { period: 3, subject_Id: "3", teacher_id: "5" },
          { period: 4, subject_Id: "4", teacher_id: "9" },
        ],
        Afternoon: [{ period: 1, subject_Id: "4", teacher_id: "2" }],
      },
      Saturday: {
        Morning: [
          { period: 1, subject_Id: "3", teacher_id: "5" },
          { period: 2, subject_Id: "4", teacher_id: "9" },
          { period: 3, subject_Id: "7", teacher_id: "4" },
          { period: 4, subject_Id: "4", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Sunday: {
        Morning: [],
        Afternoon: [],
      },
    },
    "8B": {
      Monday: {
        Morning: [
          { period: 1, subject_Id: "1", teacher_id: "3" },
          { period: 2, subject_Id: "3", teacher_id: "5" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "1", teacher_id: "9" },
          { period: 5, subject_Id: "4", teacher_id: "6" },
        ],
        Afternoon: [{ period: 1, subject_Id: "7", teacher_id: "4" }],
      },
      Tuesday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "1" },
          { period: 2, subject_Id: "3", teacher_id: "5" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "4", teacher_id: "9" },
        ],
        Afternoon: [{ period: 1, subject_Id: "4", teacher_id: "4" }],
      },
      Wednesday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "12" },
          { period: 2, subject_Id: "7", teacher_id: "14" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "3", teacher_id: "5" },
        ],
        Afternoon: [],
      },
      Thursday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "1" },
          { period: 2, subject_Id: "4", teacher_id: "9" },
          { period: 3, subject_Id: "7", teacher_id: "4" },
          { period: 4, subject_Id: "4", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Friday: {
        Morning: [
          { period: 1, subject_Id: "1", teacher_id: "3" },
          { period: 2, subject_Id: "4", teacher_id: "1" },
          { period: 3, subject_Id: "3", teacher_id: "5" },
          { period: 4, subject_Id: "4", teacher_id: "9" },
        ],
        Afternoon: [{ period: 1, subject_Id: "4", teacher_id: "2" }],
      },
      Saturday: {
        Morning: [
          { period: 1, subject_Id: "3", teacher_id: "5" },
          { period: 2, subject_Id: "4", teacher_id: "9" },
          { period: 3, subject_Id: "7", teacher_id: "4" },
          { period: 4, subject_Id: "4", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Sunday: {
        Morning: [],
        Afternoon: [],
      },
    },
    "8C": {
      Monday: {
        Morning: [
          { period: 1, subject_Id: "1", teacher_id: "3" },
          { period: 2, subject_Id: "3", teacher_id: "5" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "1", teacher_id: "9" },
          { period: 5, subject_Id: "4", teacher_id: "6" },
        ],
        Afternoon: [{ period: 1, subject_Id: "7", teacher_id: "4" }],
      },
      Tuesday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "1" },
          { period: 2, subject_Id: "3", teacher_id: "5" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "4", teacher_id: "9" },
        ],
        Afternoon: [{ period: 1, subject_Id: "4", teacher_id: "4" }],
      },
      Wednesday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "12" },
          { period: 2, subject_Id: "7", teacher_id: "14" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "3", teacher_id: "5" },
        ],
        Afternoon: [],
      },
      Thursday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "1" },
          { period: 2, subject_Id: "4", teacher_id: "9" },
          { period: 3, subject_Id: "7", teacher_id: "4" },
          { period: 4, subject_Id: "4", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Friday: {
        Morning: [
          { period: 1, subject_Id: "1", teacher_id: "3" },
          { period: 2, subject_Id: "4", teacher_id: "1" },
          { period: 3, subject_Id: "3", teacher_id: "5" },
          { period: 4, subject_Id: "4", teacher_id: "9" },
        ],
        Afternoon: [{ period: 1, subject_Id: "4", teacher_id: "2" }],
      },
      Saturday: {
        Morning: [
          { period: 1, subject_Id: "3", teacher_id: "5" },
          { period: 2, subject_Id: "4", teacher_id: "9" },
          { period: 3, subject_Id: "7", teacher_id: "4" },
          { period: 4, subject_Id: "4", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Sunday: {
        Morning: [],
        Afternoon: [],
      },
    },
  },
  "Grade 9": {
    "9A": {
      Monday: {
        Morning: [
          { period: 1, subject_Id: "1", teacher_id: "3" },
          { period: 2, subject_Id: "3", teacher_id: "5" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "1", teacher_id: "9" },
          { period: 5, subject_Id: "4", teacher_id: "6" },
        ],
        Afternoon: [{ period: 1, subject_Id: "7", teacher_id: "4" }],
      },
      Tuesday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "1" },
          { period: 2, subject_Id: "3", teacher_id: "5" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "4", teacher_id: "9" },
        ],
        Afternoon: [{ period: 1, subject_Id: "4", teacher_id: "4" }],
      },
      Wednesday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "12" },
          { period: 2, subject_Id: "7", teacher_id: "14" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "3", teacher_id: "5" },
        ],
        Afternoon: [],
      },
      Thursday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "1" },
          { period: 2, subject_Id: "4", teacher_id: "9" },
          { period: 3, subject_Id: "7", teacher_id: "4" },
          { period: 4, subject_Id: "4", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Friday: {
        Morning: [
          { period: 1, subject_Id: "1", teacher_id: "3" },
          { period: 2, subject_Id: "4", teacher_id: "1" },
          { period: 3, subject_Id: "3", teacher_id: "5" },
          { period: 4, subject_Id: "4", teacher_id: "9" },
        ],
        Afternoon: [{ period: 1, subject_Id: "4", teacher_id: "2" }],
      },
      Saturday: {
        Morning: [
          { period: 1, subject_Id: "3", teacher_id: "5" },
          { period: 2, subject_Id: "4", teacher_id: "9" },
          { period: 3, subject_Id: "7", teacher_id: "4" },
          { period: 4, subject_Id: "4", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Sunday: {
        Morning: [],
        Afternoon: [],
      },
    },
    "9B": {
      Monday: {
        Morning: [
          { period: 1, subject_Id: "1", teacher_id: "3" },
          { period: 2, subject_Id: "3", teacher_id: "5" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "1", teacher_id: "9" },
          { period: 5, subject_Id: "4", teacher_id: "6" },
        ],
        Afternoon: [{ period: 1, subject_Id: "7", teacher_id: "4" }],
      },
      Tuesday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "1" },
          { period: 2, subject_Id: "3", teacher_id: "5" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "4", teacher_id: "9" },
        ],
        Afternoon: [{ period: 1, subject_Id: "4", teacher_id: "4" }],
      },
      Wednesday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "12" },
          { period: 2, subject_Id: "7", teacher_id: "14" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "3", teacher_id: "5" },
        ],
        Afternoon: [],
      },
      Thursday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "1" },
          { period: 2, subject_Id: "4", teacher_id: "9" },
          { period: 3, subject_Id: "7", teacher_id: "4" },
          { period: 4, subject_Id: "4", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Friday: {
        Morning: [
          { period: 1, subject_Id: "1", teacher_id: "3" },
          { period: 2, subject_Id: "4", teacher_id: "1" },
          { period: 3, subject_Id: "3", teacher_id: "5" },
          { period: 4, subject_Id: "4", teacher_id: "9" },
        ],
        Afternoon: [{ period: 1, subject_Id: "4", teacher_id: "2" }],
      },
      Saturday: {
        Morning: [
          { period: 1, subject_Id: "3", teacher_id: "5" },
          { period: 2, subject_Id: "4", teacher_id: "9" },
          { period: 3, subject_Id: "7", teacher_id: "4" },
          { period: 4, subject_Id: "4", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Sunday: {
        Morning: [],
        Afternoon: [],
      },
    },
    "9C": {
      Monday: {
        Morning: [
          { period: 1, subject_Id: "1", teacher_id: "3" },
          { period: 2, subject_Id: "3", teacher_id: "5" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "1", teacher_id: "9" },
          { period: 5, subject_Id: "4", teacher_id: "6" },
        ],
        Afternoon: [{ period: 1, subject_Id: "7", teacher_id: "4" }],
      },
      Tuesday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "1" },
          { period: 2, subject_Id: "3", teacher_id: "5" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "4", teacher_id: "9" },
        ],
        Afternoon: [{ period: 1, subject_Id: "4", teacher_id: "4" }],
      },
      Wednesday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "12" },
          { period: 2, subject_Id: "7", teacher_id: "14" },
          { period: 3, subject_Id: "1", teacher_id: "8" },
          { period: 4, subject_Id: "3", teacher_id: "5" },
        ],
        Afternoon: [],
      },
      Thursday: {
        Morning: [
          { period: 1, subject_Id: "4", teacher_id: "1" },
          { period: 2, subject_Id: "4", teacher_id: "9" },
          { period: 3, subject_Id: "7", teacher_id: "4" },
          { period: 4, subject_Id: "4", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Friday: {
        Morning: [
          { period: 1, subject_Id: "1", teacher_id: "3" },
          { period: 2, subject_Id: "4", teacher_id: "1" },
          { period: 3, subject_Id: "3", teacher_id: "5" },
          { period: 4, subject_Id: "4", teacher_id: "9" },
        ],
        Afternoon: [{ period: 1, subject_Id: "4", teacher_id: "2" }],
      },
      Saturday: {
        Morning: [
          { period: 1, subject_Id: "3", teacher_id: "5" },
          { period: 2, subject_Id: "4", teacher_id: "9" },
          { period: 3, subject_Id: "7", teacher_id: "4" },
          { period: 4, subject_Id: "4", teacher_id: "4" },
        ],
        Afternoon: [],
      },
      Sunday: {
        Morning: [],
        Afternoon: [],
      },
    },
  },
};
