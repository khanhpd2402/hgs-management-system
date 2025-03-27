export const teacherData = [{ teacher_id: 1, teacher_name: "Nguyễn Văn A" }];

export const subjectData = [
  { subject_Id: 1, subject_code: "TOAN", subject_name: "Toán học" },
  { subject_Id: 11, subject_code: "TIN", subject_name: "Tin học" },
];

export const teacherSchedule = {
  "Grade 6": {
    "6A": {
      Monday: {
        Morning: [{ period: 4, subject_Id: 1, teacher_id: 1 }],
        Afternoon: [{ period: 1, subject_code: 11, teacher_id: 1 }],
      },
      Tuesday: {
        Morning: [{ period: 3, subject_Id: 1, teacher_id: 1 }],
        Afternoon: [{ period: 1, subject_Id: 1, teacher_id: 1 }],
      },
      Wednesday: {
        Morning: [{ period: 1, subject_Id: 1, teacher_id: 1 }],
        Afternoon: [],
      },
      Thursday: {
        Morning: [
          { period: 1, subject_Id: 1, teacher_id: 1 },
          { period: 3, subject_code: 11, teacher_id: 1 },
          { period: 4, subject_Id: 1, teacher_id: 1 },
        ],
        Afternoon: [],
      },
      Friday: {
        Morning: [
          { period: 1, subject_Id: 1, teacher_id: 1 },
          { period: 3, subject_Id: 1, teacher_id: 1 },
        ],
        Afternoon: [{ period: 1, subject_code: 11, teacher_id: 1 }],
      },
      Saturday: {
        Morning: [
          { period: 1, subject_Id: 1, teacher_id: 1 },
          { period: 4, subject_code: 11, teacher_id: 1 },
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
        Morning: [{ period: 4, subject_Id: 1, teacher_id: 1 }],
        Afternoon: [{ period: 1, subject_Id: 1, teacher_id: 1 }],
      },
      Tuesday: {
        Morning: [{ period: 3, subject_Id: 1, teacher_id: 1 }],
        Afternoon: [{ period: 1, subject_Id: 1, teacher_id: 1 }],
      },
      Wednesday: {
        Morning: [{ period: 1, subject_code: 11, teacher_id: 1 }],
        Afternoon: [],
      },
      Thursday: {
        Morning: [
          { period: 1, subject_Id: 1, teacher_id: 1 },
          { period: 3, subject_Id: 1, teacher_id: 1 },
          { period: 4, subject_Id: 1, teacher_id: 1 },
        ],
        Afternoon: [],
      },
      Friday: {
        Morning: [
          { period: 1, subject_code: 11, teacher_id: 1 },
          { period: 3, subject_Id: 1, teacher_id: 1 },
        ],
        Afternoon: [{ period: 1, subject_Id: 1, teacher_id: 1 }],
      },
      Saturday: {
        Morning: [
          { period: 1, subject_Id: 1, teacher_id: 1 },
          { period: 4, subject_code: 11, teacher_id: 1 },
        ],
        Afternoon: [],
      },
      Sunday: {
        Morning: [],
        Afternoon: [],
      },
    },
  },
  "Grade 7": {
    "7A": {
      Monday: {
        Morning: [{ period: 4, subject_Id: 1, teacher_id: 1 }],
        Afternoon: [{ period: 1, subject_Id: 1, teacher_id: 1 }],
      },
      Tuesday: {
        Morning: [{ period: 3, subject_Id: 1, teacher_id: 1 }],
        Afternoon: [{ period: 1, subject_code: 11, teacher_id: 1 }],
      },
      Wednesday: {
        Morning: [{ period: 1, subject_code: 11, teacher_id: 1 }],
        Afternoon: [],
      },
      Thursday: {
        Morning: [
          { period: 1, subject_code: 11, teacher_id: 1 },
          { period: 3, subject_code: 11, teacher_id: 1 },
          { period: 4, subject_Id: 1, teacher_id: 1 },
        ],
        Afternoon: [{ period: 1, subject_Id: 1, teacher_id: 1 }],
      },
      Friday: {
        Morning: [
          { period: 1, subject_Id: 1, teacher_id: 1 },
          { period: 3, subject_code: 11, teacher_id: 1 },
        ],
        Afternoon: [{ period: 1, subject_code: 11, teacher_id: 1 }],
      },
      Saturday: {
        Morning: [
          { period: 1, subject_Id: 1, teacher_id: 1 },
          { period: 4, subject_Id: 1, teacher_id: 1 },
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
        Morning: [{ period: 4, subject_code: 11, teacher_id: 1 }],
        Afternoon: [{ period: 1, subject_code: 11, teacher_id: 1 }],
      },
      Tuesday: {
        Morning: [{ period: 3, subject_Id: 1, teacher_id: 1 }],
        Afternoon: [{ period: 1, subject_Id: 1, teacher_id: 1 }],
      },
      Wednesday: {
        Morning: [{ period: 1, subject_code: 11, teacher_id: 1 }],
        Afternoon: [],
      },
    },
    "7C": {
      Monday: {
        Morning: [
          { period: 4, subject_code: 11, teacher_id: 1 },
          { period: 1, subject_Id: 1, teacher_id: 1 },
        ],
        Afternoon: [{ period: 1, subject_Id: 1, teacher_id: 1 }],
      },
      Tuesday: {
        Morning: [{ period: 3, subject_Id: 1, teacher_id: 1 }],
        Afternoon: [{ period: 1, subject_code: 11, teacher_id: 1 }],
      },
      Wednesday: {
        Morning: [{ period: 1, subject_Id: 1, teacher_id: 1 }],
        Afternoon: [],
      },
    },
  },
};
