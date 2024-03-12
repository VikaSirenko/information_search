class Theatre:
    def __init__(self, name, location, capacity, genres):
        self.name = name
        self.location = location
        self.capacity = capacity
        self.genres = genres

    def to_dict(self):
        return {
            "name": self.name,
            "location": self.location,
            "capacity": self.capacity,
            "genres": self.genres
        }
