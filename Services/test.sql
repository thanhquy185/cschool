use cschool;
SELECT act.assign_class_id, act.subject_id, ac.class_id, s.name AS subject_name, c.name AS class_name, 
       terms.name AS term_name, terms.year
FROM teachers t
JOIN assign_class_teachers act ON t.id = act.teacher_id
JOIN assign_classes ac ON act.assign_class_id = ac.id
JOIN subjects s ON act.subject_id = s.id
JOIN classes c ON ac.class_id = c.id
JOIN terms ON ac.term_id = terms.id
WHERE t.id = 1
ORDER BY terms.start_date DESC, class_name, subject_name;



--  lớp môn học của kì hiện tại
SELECT act.assign_class_id, act.subject_id, ac.class_id, s.name AS subject_name,
       c.name AS class_name, terms.name AS term_name, terms.year, t.id
FROM teachers t
JOIN assign_class_teachers act ON t.id = act.teacher_id
JOIN assign_classes ac ON act.assign_class_id = ac.id
JOIN subjects s ON act.subject_id = s.id
JOIN classes c ON ac.class_id = c.id
JOIN terms ON ac.term_id = terms.id
WHERE t.id = 10
  AND ac.term_id = (
        SELECT id
		 FROM terms     
		 WHERE status = 1 ORDER BY start_date DESC LIMIT 1
    );


SELECT s.id AS student_id, s.fullName,
FROM assign_class_teachers


-- ds học sinh lớp môn học đó 
SELECT s.id AS StudentId, s.fullName
FROM assign_classes ac
JOIN assign_class_students acs ON ac.id = acs.assign_class_id
JOIN students s ON acs.student_id = s.id
WHERE ac.id = 12